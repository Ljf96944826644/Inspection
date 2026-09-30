using HaspDemo.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Interfaces
{
    /// <summary>
    /// PLC服务契约，后续不管哪种PLC各自实现它
    /// </summary>
    public interface IPLCService
    {
        DeviceStatus Status { get; }
        event Action<DeviceStatus> StatusChanged;
        Task ConnectAsync(string ip,ushort rack,ushort slot);
        Task DisconnectAsync();

        Task<bool> ReadTriggerAsync();
        Task<ushort> ReadProductIdAsync();
        Task WriteResultAsync(bool isOk);
        Task WriteErrorCodeAsync(short code);
        Task ResetTriggerAsync();
        // <summary>断线自动重连，成功返回true。手动断开或取消时返回false</summary>
        Task<bool> TryReconnectAsync(CancellationToken ct);
    }
}
