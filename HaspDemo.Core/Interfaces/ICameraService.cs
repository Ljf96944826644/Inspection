using HaspDemo.Core.Entities;
using HaspDemo.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Interfaces
{
    /// <summary>
    /// 为相机服务契约，以后不管哪种相机各自实现它
    /// </summary>
    public interface ICameraService
    {
        DeviceStatus Status { get; }
        event Action<DeviceStatus> StatusChanged;
        /// <summary>
        /// 每帧到达事件（UI订阅做时时预览）
        /// </summary>
        event Action<ImageData> FrameArrived;
        Task ConnectAsync();
        Task DisconnectAsync();
        /// <summary>
        /// 获取最新一帧缓存（PLC触发检测的时候用）
        /// </summary>
        /// <returns></returns>
        Task<ImageData> GrabLatestAsync();
    }
}
