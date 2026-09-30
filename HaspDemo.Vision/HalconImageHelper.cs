using HalconDotNet;
using HaspDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Vision
{
    /// <summary>
    /// 图像转换助手   ImageData<->HObject
    /// </summary>
    public static class HalconImageHelper
    {
        public static HObject ToHObject(ImageData data)
        {
            var handel=GCHandle.Alloc(data.Pixels,GCHandleType.Pinned);//或者用using
            try
            {
                HOperatorSet.GenImage1(out HObject img, "byte",data.Width,data.Height,handel.AddrOfPinnedObject());
                return img;
            }
            finally
            {
                handel.Free();
            }
        }
    }
}
