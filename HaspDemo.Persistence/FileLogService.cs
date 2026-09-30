using HaspDemo.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Persistence
{
    /// <summary>
    /// 文件日志：写文件+事件广播给UI
    /// </summary>
    public class FileLogService : ILogService
    {
        private readonly object _lock = new object();
        private readonly string _logDir;
        public event Action<string> OnLog;
        public FileLogService()
        {
            _logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(_logDir))
            {
                Directory.CreateDirectory(_logDir);
            }
        }
        public void Info(string msg) => Write("INFO ", msg, null);
        public void Warn(string msg) => Write("WARN ", msg, null);
        public void Error(string msg, Exception ex = null) => Write("ERROR", msg, ex);

        private void Write(string level,string msg,Exception ex)
        {
            string line = $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {msg}"
                          + (ex == null ? "" : $" | {ex.Message}");
            lock (_lock)
            {
                File.AppendAllText(Path.Combine(_logDir, $"{DateTime.Now:yyyy-MM-dd}.log"), line + Environment.NewLine);
            }
            OnLog?.Invoke(line);
        }
    }
}