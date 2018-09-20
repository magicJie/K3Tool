using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTool.Batcher
{
    public class SqlBatcher : AbstractBatcher
    {
        private SqlConnection _dbConnection;
        private const char ParameterChar = '@';

        public override DbConnection DbConnection
        {
            get { return _dbConnection; }
            set { _dbConnection = (SqlConnection) value; }
        }

        public override void Insert(DataTable dt,string targetTableName, Dictionary<string, string> mapping)
        {
            _dbConnection.Open();
            var tx = _dbConnection.BeginTransaction();
            var copy = new SqlBulkCopy(_dbConnection, SqlBulkCopyOptions.CheckConstraints, tx)
            {
                BatchSize = dt.Rows.Count,
                DestinationTableName = targetTableName
            };
            foreach (string key in mapping.Keys)
            {
                copy.ColumnMappings.Add(key,mapping[key]);
            }
            try
            {
                copy.WriteToServer(dt);
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                _dbConnection.Close();
            }
            base.Insert(dt,targetTableName,mapping);
        }

        public override void Update(DataTable dt, string targetTableName, Dictionary<string, string> mapping)
        {
            _dbConnection.Open();
            var tx = _dbConnection.BeginTransaction();
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = _dbConnection
                };
                var targetColumnNames = new List<string>();
                var cmdText = $@"update {targetTableName} set ";
                //有映射规则应用映射规则，否则采用默认列名
                for (int i = 0, j = dt.Columns.Count; i < j; i++)
                {
                    var parameter = new SqlParameter();
                    var targetColName = mapping.ContainsKey(dt.Columns[i].ColumnName)
                        ? mapping[dt.Columns[i].ColumnName]
                        : dt.Columns[i].ColumnName;
                    parameter.ParameterName = targetColName;
                    cmd.Parameters.Add(parameter);
                    cmdText += $@"{targetColName} = {ParameterChar}{targetColName}";
                    if (i != j - 1)
                        cmdText += ",";
                }
                cmdText += " where ";
                for (int i = 0, j = dt.PrimaryKey.Length; i < j; i++)
                {
                    var targetColName = mapping.ContainsKey(dt.Columns[i].ColumnName)
                        ? mapping[dt.Columns[i].ColumnName]
                        : dt.Columns[i].ColumnName;
                    cmdText += $@"{targetColName} = {ParameterChar}{targetColName}";
                    if (i != j - 1)
                        cmdText += " and ";
                }
                cmd.CommandText = cmdText;
                cmd.Prepare();
                foreach (DataRow dataRow in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        cmd.Parameters[i].Value = dataRow[i];
                    }
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                _dbConnection.Close();
            }
            base.Update(dt, targetTableName, mapping);
        }

        public override void Delete(DataTable dt, string targetTableName)
        {
            _dbConnection.Open();
            var tx = _dbConnection.BeginTransaction();
            try
            {
                var cmd = new SqlCommand()
                {
                    Connection = _dbConnection
                };
                var cmdText = $@"delete from {targetTableName} where ";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    cmdText += dt.Columns[i].ColumnName + " = " + ParameterChar + dt.Columns[i].ColumnName;
                    if (i != dt.Columns.Count - 1)
                    {
                        cmdText += " and ";
                    }
                    var paramater=new SqlParameter();
                    paramater.ParameterName = dt.Columns[i].ColumnName;
                }
                cmd.CommandText = cmdText;
                cmd.Prepare();
                foreach (DataRow dataRow in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        cmd.Parameters[i].Value = dataRow[i];
                    }
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                throw;
            }
            finally
            {
                _dbConnection.Close();
            }
            base.Delete(dt, targetTableName);
        }
    }
}
