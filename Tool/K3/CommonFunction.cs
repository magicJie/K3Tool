using System;
using System.Collections.Generic;
using Tool.Common;
using Tool.Sql;

namespace Tool.K3
{
    public static class CommonFunction
    {
        /// <summary>
        /// 根据物料属性，找物料内码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="columnname"></param>        
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetMaterielIsnByCode(string conn,string columnname,string value)
        {
            string sqlstring = string.Format("select fitemid from t_ICItemCore where {0}='{1}'", columnname, value);
            var datatable = SqlHelper.Query(conn, sqlstring);
            if (datatable.Rows.Count > 0)
            {
                return datatable.Rows[0][0].ToString();
            }
            else
            {
                throw new Exception("物料:" + value + "不存在！");
            }
        }
        /// <summary>
        /// 获取某个表的最大编码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public static int GetMaxNum(string conn, string tablename)
        {
            string sqlstring = string.Format("select FMaxNum from ICMaxNum where FTableName='{0}'", tablename);
            var datatable = SqlHelper.Query(conn, sqlstring);
            if (datatable.Rows.Count > 0)
            {
                return int.Parse(datatable.Rows[0][0].ToString());
            }
            else
            {
                throw new Exception("找不到" + tablename + "的最大编码！");
            }
        }

        public static void UpdateMaxNum(string conn, string tablename, int number)
        {
            List<string> sqlList=new List<string>();
            string sqlstring = string.Format("update ICMaxNum set FMaxNum={1} where FTableName='{0}'", tablename,number);
            sqlList.Add(sqlstring);
            SqlHelper.ExecuteSqlTran(conn, sqlList);
        }
        public static List<string> GetSqlList<T>(string conn, List<T> list, string tablename)
        {
            var headdatatable = DataTableTool.GetDataTableByClass(list);
            return DataTableTool.DataTableToInsertSql(headdatatable, tablename);            
        }
        /// <summary>
        /// kindeestate 1是已导入数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tablename"></param>
        public static void Initalize(string conn,string tablename)
        {
            List<string> sqlList=new List<string>();
            string sqlstring =
                string.Format(
                    "if not exists(select * from syscolumns where id=object_id('{0}') and name='kindeestate') alter table {0} add kindeestate int", tablename);            
            sqlList.Add(sqlstring);
            SqlHelper.ExecuteSqlTran(conn, sqlList);
        }

        
    }
}
