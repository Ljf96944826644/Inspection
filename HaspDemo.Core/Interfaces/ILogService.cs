using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Interfaces
{
    /// <summary>
    /// 日志契约
    /// </summary>
    public interface ILogService
    {
        event Action<string> OnLog;
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg,Exception ex);
    }
}
