using System;
using System.Collections.Generic;
using Tool.Common;
using Tool.Sql;

namespace Tool.K3
{
    public static class CommonFunction
    {
        /// <summary>
        /// 根据属性，找金蝶内码
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="fitemclassid"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string Getfitemid(string conn,Fitemclassid fitemclassid, string where)
        {
            int type = 0;            
            switch (fitemclassid)
            {
                case Fitemclassid.客户:
                    type = 1;
                    break;
                case Fitemclassid.部门:
                    type = 2;
                    break;
                case Fitemclassid.职员:
                    type = 3;
                    break;
                case  Fitemclassid.物料:
                    type = 4;
                    break;
                case Fitemclassid.仓库:
                    type = 5;
                    break;               
                case Fitemclassid.单位:
                    type = 7;
                    break;
                case Fitemclassid.供应商:
                    type = 8;
                    break;
                case Fitemclassid.医师:
                    type = 3003;
                    break;
                case Fitemclassid.登录用户:
                    type = 9999;
                    break;

            }
            string sqlstring = string.Format("select FItemID from t_item where FItemClassID='{0}' and {1}", type,where);
            if (type == 9999)
            {
                sqlstring = string.Format("select FUserID from t_user where {0}", where);
            } 
            var datatable = SqlHelper.Query(conn, sqlstring);
            if (datatable.Rows.Count > 0)
            {
                return datatable.Rows[0][0].ToString();
            }
            log4net.LogManager.GetLogger("logger").Error(sqlstring);
            throw new Exception(fitemclassid+":"+@where+"不存在！");
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
