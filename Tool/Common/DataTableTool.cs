using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Tool.Common
{
    public static class DataTableTool
    {
        /// <summary>
        /// 根据指定类型的实例集合返回datatable
        /// </summary>
        /// <typeparam name="T">实例类型</typeparam>
        /// <param name="tList">实例集合</param>
        /// <returns></returns>
        public static DataTable GetDataTableByClass<T>(List<T> tList)
        {
            DataTable dataTable = new DataTable();
            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                dataTable.Columns.Add(info.Name, info.PropertyType);
            }
            foreach (var item in tList)
            {
                DataRow newrow = dataTable.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    newrow[info.Name] = info.GetValue(item, null);
                }
                dataTable.Rows.Add(newrow);
            }
            return dataTable;
        }

        /// <summary>
        /// 根据datatable返回insert语句
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static List<string> DataTableToInsertSql(DataTable dataTable, string tablename)
        {
            List<string> sqlStrs = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                string columns = null;
                string values = "'";
                foreach (DataColumn column in dataTable.Columns)
                {
                    columns = columns + column.ColumnName + ",";
                    values = values + row[column.ColumnName] + "','";
                }
                if (columns != null)
                {
                    columns = columns.TrimEnd(",".ToCharArray());
                }
                values = values.TrimEnd("'".ToCharArray());
                values = values.TrimEnd(",".ToCharArray());
                sqlStrs.Add(string.Format("insert into {0}({1}) values({2})", tablename, columns, values));
            }
            return sqlStrs;
        }
    }
}
