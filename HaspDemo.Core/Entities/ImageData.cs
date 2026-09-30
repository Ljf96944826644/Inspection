using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Entities
{
    public class ImageData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public byte[] Pixels { get; set; }//Mono8灰度像素
        public DateTime GrabTime { get; set; }//采集时间
    }
}
