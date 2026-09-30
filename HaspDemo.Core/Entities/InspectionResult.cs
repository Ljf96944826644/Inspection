using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Entities
{
    /// <summary>
    /// 单项检测结果（表格里面的一行）
    /// </summary>
    public class CheckItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsOk { get; set; }
    }
    /// <summary>
    /// 一帧图像的完整的检测结果
    /// </summary>
    public class InspectionResult
    {

        public List<CheckItem> Items { get; set; } = new List<CheckItem>();
        public List<double[]> ScratchPoints { get; } = new List<double[]>();
        public List<double[]> Matches { get; } = new List<double[]>();
        public double ElapseMs { get; set; } 
        public bool IsAllOk
        {
            get
            {
                foreach (var item in Items)
                {
                    if (!item.IsOk)
                    {
                        return false;
                    }
                }   return true;
            }
        }
        /// <summary>
        /// 第一个不合格项的名字，如果是全部OK就返回null
        /// </summary>
        public string FirstNgItem
        {
            get
            {
                foreach(var item in Items)
                {
                    if (!item.IsOk)
                    {
                        return item.Name;
                    }
                   
                }
                return null;
            }
        }
    }
   
        
}
