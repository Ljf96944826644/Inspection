using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HaspDemo.Core.Entities;
using HaspDemo.Core.Enums;
using HaspDemo.Core.Interfaces;
using HaspDemo.Vision;

namespace HaspDemo.Applicationsss
{
    /// <summary>
    /// 检测业务服务：轮询PLC触发 → 取图 → 检测 → 写回PLC → 存数据库
    /// 它只认识 Core 接口，不知道相机是海康、PLC是西门子、算法是Halcon
    /// </summary>
    public class DetectionService
    {
        private readonly ICameraService _camera;
        private readonly IPLCService _plc;
        private readonly IDetectAlgorithm _algorithm;
        private readonly IDetectRecordRepository _repository;
        private readonly ILogService _log;

        // 算法层的具体类型引用（保存图像需要调 HalconWasherInspector.SaveImage）
        // 这里有点妥协——更严谨的做法是把保存图像也定义到接口里
        private readonly HalconWasherInspector _halconInspector;

        private CancellationTokenSource _cts;

        /// <summary>
        /// 对外暴露的属性，外部只能用不能改
        /// </summary>
        public bool IsRunning => _cts != null;

        public DeviceStatus CameraStatus => _camera.Status;
        public DeviceStatus PLCStatus => _plc.Status;
        public ICameraService Camera => _camera;
        public IPLCService PLC => _plc;

        /// <summary>每完成一帧检测结果触发（UI订阅刷新界面）</summary>
        public event Action<InspectionResult, ushort, string> DetectionCompleted;

        public DetectionService(ICameraService camera, IPLCService plc,
            IDetectAlgorithm algorithm, IDetectRecordRepository repository,
            ILogService log, HalconWasherInspector halconInspector)
        {
            _camera = camera;
            _plc = plc;
            _algorithm = algorithm;
            _repository = repository;
            _log = log;
            _halconInspector = halconInspector;
        }

        // ==================== 启动/停止轮询 ====================

        public void StartPolling(string plcIp, ushort rack, ushort slot)
        {
            if (IsRunning) return;

         
            if (_camera.Status != DeviceStatus.Connected)
            {
                _log.Warn("启动失败：相机未连接，请先连接相机");
                return;    
            }
            if (_plc.Status != DeviceStatus.Connected)
            {
                _log.Warn("启动失败：PLC未连接，请先连接PLC");
                return;
            }
            _cts = new CancellationTokenSource();
            _ = PollLoopAsync(plcIp, rack, slot, _cts.Token);
            _log.Info("检测循环已启动，等待PLC触发...");
        }

        public void StopPolling()
        {
            if (_cts == null) return;
            _cts.Cancel();
            _cts = null;
            _log.Info("检测循环已停止");
        }

        // ==================== 轮询主循环 ====================

        // ==================== 轮询主循环 ====================

        private async Task PollLoopAsync(string ip, ushort rack, ushort slot, CancellationToken ct)
        {
            

            bool lastTrigger = false;
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    bool trigger = await _plc.ReadTriggerAsync();

                    // 检测上升沿
                    if (trigger && !lastTrigger)
                    {
                        ushort productId = await _plc.ReadProductIdAsync();
                        _log.Info($"检测到触发！ProductID={productId}");

                        await HandleTriggerAsync(productId, ct);

                        await _plc.ResetTriggerAsync();
                        _log.Info("Trigger 已复位，等待下一个工件");
                    }
                    lastTrigger = trigger;
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _log.Error("通讯异常", ex);

                    // 断线重连：直接调接口，不再需要 dynamic
                    bool ok = await _plc.TryReconnectAsync(ct);
                    if (!ok)
                    {
                        _log.Warn("重连失败或手动断开，退出轮询");
                        break;
                    }
                    lastTrigger = false;   // 重连后清边沿记忆，避免误触发
                }
            }
        }






        // ==================== 单次触发处理 ====================

        private async Task HandleTriggerAsync(ushort productId, CancellationToken ct)
        {
            // 1. 取最新帧
            ImageData image;
            try
            {
                image = await _camera.GrabLatestAsync();
            }
            catch (Exception ex)
            {
                _log.Error("检测失败：无可用图像",ex);
                await _plc.WriteResultAsync(false);
                await _plc.WriteErrorCodeAsync(99);
                return;
            }

            // 2. 算法检测
            InspectionResult result;
            string savedImagePath = null;
            try
            {
                result = await _algorithm.DetectAsync(image, ct);

                // 保存图像（调Vision层提供的方法）
                var hImg = HalconImageHelper.ToHObject(image);
                try { savedImagePath = _halconInspector.SaveImage(hImg, productId); }
                finally { hImg?.Dispose(); }

                _log.Info($"检测完成：{(result.IsAllOk ? "OK" : "NG")}，耗时 {result.ElapseMs:F0}ms");
            }
            catch (Exception ex)
            {
                _log.Error("检测异常", ex);
                await _plc.WriteResultAsync(false);
                await _plc.WriteErrorCodeAsync(99);
                return;
            }

            // 3. 映射错误码（业务规则属于这一层）
            short errorCode = 0;
            if (!result.IsAllOk)
            {
                switch (result.FirstNgItem)
                {
                    case "模板匹配": errorCode = 1; break;
                    case "尺寸测量": errorCode = 2; break;
                    case "划痕检测": errorCode = 3; break;
                    default: errorCode = 9; break;
                }
            }

            // 4. 回写PLC
            try
            {
                await _plc.WriteResultAsync(result.IsAllOk);
                await _plc.WriteErrorCodeAsync(errorCode);
                _log.Info($"已回写结果：{(result.IsAllOk ? "OK" : "NG")}，错误码={errorCode}");
            }
            catch (Exception ex)
            {
                _log.Error("写PLC失败", ex);
            }

            // 5. 存数据库（失败不影响生产）
            try
            {
                await _repository.InsertAsync(new InspectRecord
                {
                    CheckTime = DateTime.Now,
                    ProductId = productId,
                    Result = result.IsAllOk ? "OK" : "NG",
                    Measurements = BuildMeasurements(result),
                    DefectInfo = BuildDefects(result),
                    ImagePath = savedImagePath ?? ""
                });
                _log.Info("记录已存入数据库");
            }
            catch (Exception ex)
            {
                _log.Error("数据库保存失败", ex);
            }

            // 6. 通知UI
            DetectionCompleted?.Invoke(result, productId, savedImagePath);
        }

        private string BuildMeasurements(InspectionResult r)
        {
            var items = r.Items.Where(x => x.Name.Contains("尺寸") || x.Name.Contains("测量"));
            return string.Join("; ", items.Select(x => $"{x.Name}={x.Value}"));
        }

        private string BuildDefects(InspectionResult r)
        {
            var items = r.Items.Where(x => x.Name.Contains("划痕") || x.Name.Contains("缺陷"));
            return items.Any() ? string.Join("; ", items.Select(x => $"{x.Name}={x.Value}")) : "无";
        }
    }
}
