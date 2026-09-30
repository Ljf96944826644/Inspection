using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Constants
{
    public static  class PlcAddress
    {
        public const string Trigger = "DB2.DBX0.0";  // 触发信号（上升沿）
        public const string ProductId = "DB2.DBW2";    // 产品ID
        public const string ResultOk = "DB2.DBX4.0";  // 检测结果
        public const string ErrorCode = "DB2.DBW6";    // 错误码
    }
}
