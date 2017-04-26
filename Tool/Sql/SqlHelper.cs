using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Tool.Sql
{
    public static class SqlHelper
    {       
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="con">链接字符串</param>
        /// <param name="sqlStringList">多条SQL语句</param>		
        public static int ExecuteSqlTran(string con, List<String> sqlStringList)
        {
            using (var conn = new SqlConnection(con))
            {
                conn.Open();
                var cmd = new SqlCommand {Connection = conn};
                var tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    var count = 0;
                    foreach (string strsql in sqlStringList)
                    {
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch(Exception ex)
                {
                    log4net.LogManager.GetLogger("logger").Error(cmd.CommandText);
                    tx.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回DataTable
        /// </summary>
        /// <param name="conn">连接字符串</param>
        /// <param name="sqlString">sql语句</param>
        /// <param name="distan"></param>
        /// <returns></returns>
        public static DataTable Query(string conn, string sqlString,bool distan=false)
        {

            using (var connection = new SqlConnection(conn))
            {
                var ds = new DataSet();
                try
                {
                    connection.Open();
                    var command = new SqlDataAdapter(sqlString, connection);
                    command.Fill(ds, "ds");
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                if (distan)
                {
                    return DistanTable(ds.Tables[0]);
                }
                return ds.Tables[0];
            }

        }

        public static DataTable DistanTable(DataTable dataTable)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn column in dataTable.Columns)
            {
                columns.Add(column.ColumnName);
            }
            DataView dataView=new DataView(dataTable);
            return dataView.ToTable(true, columns.ToArray());
        }
    }
}
