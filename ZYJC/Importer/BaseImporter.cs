using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZYJC.Model;

namespace ZYJC.Importer
{
    public class BaseImporter
    {
        public readonly string SourceConnStr = ConfigurationManager.AppSettings["source"];
        public readonly string RelatedConnStr = ConfigurationManager.AppSettings["related"];

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns>导入行数</returns>
        public virtual int Import()
        {
            return 0;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>导入行数</returns>
        public virtual int Import(DateTime startTime,DateTime endTime)
        {
            return 0;
        }
    }
}