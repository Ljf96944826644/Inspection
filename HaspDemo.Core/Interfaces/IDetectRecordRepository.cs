using HaspDemo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaspDemo.Core.Interfaces
{
    /// <summary>
    /// 数据库仓库契约
    /// </summary>
    public interface IDetectRecordRepository
    {
        Task InsertAsync(InspectRecord record);
    }
}
