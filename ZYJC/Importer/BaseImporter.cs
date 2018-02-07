using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using ZYJC.Model;

namespace ZYJC.Importer
{
    public class BaseImporter
    {
        public readonly SqlConnection SourceConn = new SqlConnection(ConfigurationManager.ConnectionStrings["source"].ConnectionString);
        public readonly OracleConnection RelatedConn =new OracleConnection(ConfigurationManager.ConnectionStrings["related"].ConnectionString);
        public const int BatchNum = 2000;

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>导入行数</returns>
        public virtual int InitImport(DateTime startTime,DateTime endTime)
        {
            return 0;
        }

        public virtual int UpdateImport(DateTime startTime, DateTime endTime)
        {
            return 0;
        }
    }
}