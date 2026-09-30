using HaspDemo.Core.Constants;
using HaspDemo.Core.Enums;
using HaspDemo.Core.Interfaces;
using S7.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HaspDemo.Devices
{
    /// <summary>
    /// S7-1500 PLC实现，内置自动重连逻辑，上层不需要关心断线
    /// </summary>
    public class S7Plc : IPLCService
    {
        private Plc _plc;
        private string _ip;
        private ushort _rack, _slot;
        private bool _manualDisconnect;

        public DeviceStatus Status { get; private set; } = DeviceStatus.Disconnected;
        public event Action<DeviceStatus> StatusChanged;

        private void SetStatus(DeviceStatus s)
        {
            if (Status == s) return;          // ★ 状态没变就不重复发事件
            Status = s;
            StatusChanged?.Invoke(Status);
        }

        public async Task ConnectAsync(string ip, ushort rack, ushort slot)
        {
            // 幂等：已经连着就直接返回（UI重复点击、轮询循环再连都安全）
            if (Status == DeviceStatus.Connected && IsConnected())
                return;

            _ip = ip;
            _rack = rack;
            _slot = slot;
            _manualDisconnect = false;
            SetStatus(DeviceStatus.Conneting);   // 枚举名如果叫 Conneting 就用你的原名
            try
            {
                _plc?.Close();                  // 上次残留的连接对象清掉
                _plc = new Plc(CpuType.S71500, ip, (short)rack, (short)slot);
                await _plc.OpenAsync();

                SetStatus(DeviceStatus.Connected);   // ★★★ 核心修复：成功 → Connected
            }
            catch
            {
                SetStatus(DeviceStatus.Disconnected);  // ★ 失败回退，别卡在 Connecting
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            _manualDisconnect = true;
            await Task.Run(() =>
            {
                try { _plc?.Close(); } catch { }
                _plc = null;
            });
            SetStatus(DeviceStatus.Disconnected);
        }

        public async Task<bool> ReadTriggerAsync()
        {
            var obj = await _plc.ReadAsync(PlcAddress.Trigger);
            return (bool)obj;
        }

        public async Task<ushort> ReadProductIdAsync()
        {
            var obj = await _plc.ReadAsync(PlcAddress.ProductId);
            return (ushort)obj;
        }

        public async Task WriteErrorCodeAsync(short code)
        {
            await _plc.WriteAsync(PlcAddress.ErrorCode, code);
        }

        public async Task WriteResultAsync(bool isOk)
        {
            await _plc.WriteAsync(PlcAddress.ResultOk, isOk);
        }

        public async Task ResetTriggerAsync()
        {
            await _plc.WriteAsync(PlcAddress.Trigger, false);
        }

        /// <summary>
        /// 断线自动重连（给Application层调用，业务循环不用管重连）
        /// </summary>
        public async Task<bool> TryReconnectAsync(CancellationToken ct)
        {
            if (_manualDisconnect)
                return false;

            while (!ct.IsCancellationRequested && !_manualDisconnect)
            {
                try
                {
                    await Task.Delay(2000, ct);
                    SetStatus(DeviceStatus.Conneting);
                    _plc?.Close();
                    _plc = new Plc(CpuType.S71500, _ip, (short)_rack, (short)_slot);
                    await _plc.OpenAsync();
                    SetStatus(DeviceStatus.Connected);
                    return true;
                }
                catch
                {
                    SetStatus(DeviceStatus.Disconnected);   // 重试期间UI能看到状态变化
                    /* 两秒后重试 */
                }
            }
            return false;
        }

        public bool IsConnected()
        {
            return _plc != null && _plc.IsConnected;
        }
    }
}
