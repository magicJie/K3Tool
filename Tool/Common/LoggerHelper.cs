using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Common
{
    public class LoggerHelper
    {
        /// <summary>
        /// 导入数据时记录有问题的数据
        /// </summary>
        /// <param name="sourceTableName">目标表名</param>
        /// <param name="keyName">目标表主键列名</param>
        /// <param name="keyValue">出错行主键值</param>
        /// <param name="badData">出错的列及错误值、替代值</param>
        public static void WriteWarnInfo(string sourceTableName,string keyName,string keyValue,List<Tuple<string,string,string>> badData)
        {
            var log = log4net.LogManager.GetLogger("logger");
            log.Warn(string.Format("警告：导入了问题数据。sourceTableName:{0},keyName:{1},keyValue:{2}。badData:{3}",
                sourceTableName,keyName,keyValue,string.Join(",", badData.Select(x => "(" + x.Item1 + "，" + x.Item2+","+x.Item3+")"))));
        }

        /// <summary>
        /// 检查一行数据中是否存在有数值为空的数据
        /// </summary>
        /// <param name="tableName">待检查的表名</param>
        /// <param name="dataRow">待检查的行</param>
        /// <param name="keyColumnName">主键列名</param>
        /// <param name="columnNames">要检查的列名</param>
        /// <returns>true:存在空值列,false:不存在空值列</returns>
        public static bool CheckValue(string tableName, DataRow dataRow,string keyColumnName,params string[] columnNames)
        {
            var result = false;
            for (int i = 0; i < dataRow.ItemArray.Length; i++)
            {
                var item = dataRow[i];
                if ((item == null || string.IsNullOrWhiteSpace(item.ToString())) &&
                    columnNames.Any(x=>x.Equals(dataRow.Table.Columns[i].ColumnName,StringComparison.OrdinalIgnoreCase)))
                {
                    var log = log4net.LogManager.GetLogger("logger");
                    log.Warn(string.Format("导入源数据表【{0}】时,列【{1}】的值为空。主键【{2}】的值为【{3}】",
                        tableName, dataRow.Table.Columns[i].ColumnName,keyColumnName,dataRow[keyColumnName]));
                    result = true;
                }
            }
            return result;
        }
    }
}
