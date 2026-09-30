using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Entities
{
    /// <summary>
    /// 对应数据库表
    /// </summary>
    public class InspectRecord
    {
        public int Id { get; set; }
        public DateTime CheckTime { get; set; }
        public int ProductId { get; set; }
        public string Result { get; set; }       // "OK"/"NG"
        public string Measurements { get; set; }
        public string DefectInfo { get; set; }
        public string ImagePath { get; set; }
    }
}
