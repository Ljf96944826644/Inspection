using HaspDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Interfaces
{
    /// <summary>
    /// 检测算法契约，以后换算法久换实现
    /// </summary>
    public interface IDetectAlgorithm
    {
        Task<InspectionResult> DetectAsync(ImageData image, CancellationToken ct);
    }
}
