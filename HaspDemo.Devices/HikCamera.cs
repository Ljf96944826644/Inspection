using HaspDemo.Core.Entities;
using HaspDemo.Core.Interfaces;
using HaspDemo.Core.Enums;
using MvCameraControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Devices
{
    /// <summary>
    /// 海康相机实现：连续读取+最新帧缓存+FrameArrived事件广播
    /// </summary>
    public class HikCamera:ICameraService
    {
        private readonly object _frameLock = new object();
        private IDevice _device;//来自MvCameraControl
        private ImageData _latestFrame;
        private CancellationTokenSource _grabCts;
        private Task _grabTask;


        public DeviceStatus Status { get; private set; } = DeviceStatus.Disconnected;
        public event Action<DeviceStatus> StatusChanged;
        public event Action<ImageData> FrameArrived;
        private void SetStatus(DeviceStatus s)
        {
            Status=s;
            StatusChanged?.Invoke(s);
        }

        public async Task ConnectAsync()
        {
            if (Status == DeviceStatus.Connected)
                return;

            SetStatus(DeviceStatus.Conneting);
            try
            {
                // ★ 内核不抛异常，只返回错误消息（null=成功）
                string err = await Task.Run(() => ConnectCore());

                if (err != null)
                    throw new Exception(err);   //  在await之后抛：调试器能追踪到UI的catch，不中断

                SetStatus(DeviceStatus.Connected);
            }
            catch
            {
                SetStatus(DeviceStatus.Disconnected);
                throw;
            }
        }

        /// <summary>干活内核：返回null=成功，否则返回错误消息。在后台线程执行</summary>
        private string ConnectCore()
        {
            // 1.枚举设备
            int ret = DeviceEnumerator.EnumDevices(
                DeviceTLayerType.MvGigEDevice | DeviceTLayerType.MvUsbDevice,
                out var devicelist);
            if (ret != MvError.MV_OK)
                return $"枚举相机失败，错误码 {ret}";

            // 2.数量检查
            if (devicelist.Count == 0)
                return "未找到相机！请检查：① 相机电源是否上电 ② 网线是否插好 ③ 相机IP和电脑是否在同一网段（海康默认192.168.x.x）";

            // 3.打开设备
            _device = DeviceFactory.CreateDevice(devicelist[0]);
            ret = _device.Open();
            if (ret != MvError.MV_OK)
                return $"相机打开失败，错误码 {ret}";

            // 4.配置相机
            _device.Parameters.SetEnumValueByString("AcquisitionMode", "Continuous");
            _device.Parameters.SetEnumValueByString("TriggerMode", "Off");
            _device.Parameters.SetEnumValueByString("PixelFormat", "Mono8");
            _device.Parameters.SetEnumValueByString("ExposureTime", "12000f");

            // 5.启动取流线程
            _grabCts = new CancellationTokenSource();
            _grabTask = Task.Run(() => GrabLoop(_grabCts.Token));

            return null;   // ★ 成功
        }



        public async Task DisconnectAsync()
        {
           _grabCts?.Cancel();
            try
            {
                _grabTask?.Wait(2000);
            }
            catch
            {

                
            }
            _grabCts?.Dispose();
            _grabCts=null;
            await Task.Run(() =>
            {
                try
                {
                    _device?.StreamGrabber.StopGrabbing();
                    _device?.Close();
                    _device?.Dispose();
                }
                catch 
                {

                    _device = null;
                    _latestFrame = null;
                }
            });
            SetStatus(DeviceStatus.Disconnected);
        }
        public  Task<ImageData> GrabLatestAsync()
        {
            lock (_frameLock)
            {
                if(_latestFrame == null)
                {
                    throw new Exception("相机还没有采集到任何图像");
                }
                //返回副本，避免和后台取流线程共享引用
               return Task.FromResult(new ImageData
                {
                    Width = _latestFrame.Width,
                    Height = _latestFrame.Height,
                    Pixels = (byte[])_latestFrame.Pixels.Clone(),
                    GrabTime = _latestFrame.GrabTime
                });
            }
        }
        /// <summary>
        /// 循环取流
        /// </summary>
        /// <param name="ct"></param>
        public void GrabLoop(CancellationToken ct)
        {
            int ret=_device.StreamGrabber.StartGrabbing();
            if (ret != MvError.MV_OK)
            {
                SetStatus(DeviceStatus.Error);
                return; 
            }
            while (!ct.IsCancellationRequested)
            {
                ret=_device.StreamGrabber.GetImageBuffer(1000,out IFrameOut frame);
                if (ret != MvError.MV_OK)
                {
                    Thread.Sleep(5);
                    continue;
                }
                try
                {
                    var image = frame.Image;
                    var data = new ImageData
                    {
                        Width = (int)image.Width,
                        Height = (int)image.Height,
                        Pixels = (byte[])image.PixelData,
                        GrabTime = DateTime.Now

                    };
                    lock (_frameLock)
                    {
                        _latestFrame= data;
                    }
                    FrameArrived?.Invoke(data);
                }
                finally
                {
                    _device.StreamGrabber.FreeImageBuffer(frame);
                }
            }
        }

    }
}
