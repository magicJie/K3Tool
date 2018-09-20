using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace DataTool.Batcher
{
    public class OracleBatcher : AbstractBatcher
    {
        private OracleConnection _dbConnection;
        private const char ParameterChar = ':';
        public override DbConnection DbConnection
        {
            get { return _dbConnection; }

            set { _dbConnection = (OracleConnection)value; }
        }

        public override void Insert(DataTable dt, string targetTableName, Dictionary<string, string> mapping)
        {
            _dbConnection.Open();
            var tx = _dbConnection.BeginTransaction();
            try
            {
                var cmd = new OracleCommand()
                {
                    Connection = _dbConnection
                };
                var targetColumnNames = new List<string>();
                //有映射规则应用映射规则，否则采用默认列名
                foreach (DataColumn column in dt.Columns)
                {
                    var parameter = new OracleParameter();
                    if (mapping.ContainsKey(column.ColumnName))
                    {
                        targetColumnNames.Add(mapping[column.ColumnName]);
                        parameter.ParameterName = mapping[column.ColumnName];
                    }
                    else
                    {
                        targetColumnNames.Add(column.ColumnName);
                        parameter.ParameterName = column.ColumnName;
                    }
                    cmd.Parameters.Add(parameter);
                }
                var cmdText = $@"insert into {targetTableName} ({string.Join(",", targetColumnNames)}) values({string.Join(",", targetColumnNames.Select(x => ParameterChar + x))})";
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
            base.Insert(dt, targetTableName, mapping);
        }

        public override void Update(DataTable dt, string targetTableName, Dictionary<string, string> mapping)
        {
            _dbConnection.Open();
            var tx = _dbConnection.BeginTransaction();
            try
            {
                var cmd = new OracleCommand()
                {
                    Connection = _dbConnection
                };
                var targetColumnNames = new List<string>();
                var cmdText = $@"update {targetTableName} set ";
                //有映射规则应用映射规则，否则采用默认列名
                for (int i = 0, j = dt.Columns.Count; i < j; i++)
                {
                    var parameter = new OracleParameter();
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
                var cmd = new OracleCommand()
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
                    var parameter=new OracleParameter();
                    parameter.ParameterName = dt.Columns[i].ColumnName;
                    cmd.Parameters.Add(parameter);
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