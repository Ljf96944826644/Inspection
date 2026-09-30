using HaspDemo.Core.Entities;
using HaspDemo.Core.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;//ExecuteAsync

namespace HaspDemo.Persistence
{
    /// <summary>
    /// 检测记录仓库
    /// </summary>
    public class InspectRecordRepository : IDetectRecordRepository
    {
        private readonly string _connStr;
        public InspectRecordRepository(string connStr)
        {
            _connStr = connStr;
        }
        public async Task InsertAsync(InspectRecord record)
        {
            const string sql = @"INSERT INTO InspectionRecords(CheckTime, ProductId, Result, Measurements, DefectInfo, ImagePath)VALUES(@CheckTime, @ProductId, @Result, @Measurements, @DefectInfo, @ImagePath)";
            using (var conn=new SqlConnection(_connStr))
            {
                await conn.ExecuteAsync(sql, record);
            }
        }


    }
}