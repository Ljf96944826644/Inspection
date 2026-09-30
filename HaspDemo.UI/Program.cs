using System;
using System.Windows.Forms;
using HaspDemo.Applicationsss;
using HaspDemo.Core.Interfaces;
using HaspDemo.Devices;
using HaspDemo.Persistence;
using HaspDemo.UI;
using HaspDemo.Vision;
using Microsoft.Extensions.DependencyInjection;
using MvCameraControl;
namespace HaspDemo.UI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            SDKSystem.Initialize();   // 海康SDK全局初始化
            
            var services=new ServiceCollection();
            services.AddSingleton<ILogService, FileLogService>();
            services.AddSingleton<ICameraService, HikCamera>();
            services.AddSingleton<IPLCService, S7Plc>();
            services.AddSingleton<HalconWasherInspector>();
            services.AddSingleton<IDetectAlgorithm>(sp => sp.GetRequiredService<HalconWasherInspector>());
            services.AddSingleton<IDetectRecordRepository>(
                new InspectRecordRepository(
                    "Data Source=WIN-3LH90C0IBON;Initial Catalog=HalconProject;Integrated Security=True;TrustServerCertificate=True;"));
            services.AddSingleton<DetectionService>();

            var provider = services.BuildServiceProvider();

            Application.Run(new MainForm(
                provider.GetRequiredService<DetectionService>(),
                provider.GetRequiredService<ILogService>()));

            SDKSystem.Finalize();   // 程序退出前释放
        }
    }
}