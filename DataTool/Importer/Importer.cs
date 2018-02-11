using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Oracle.DataAccess.Client;
using DataTool.Model;

namespace DataTool
{
    public class Importer
    {
        public readonly SqlConnection SourceConn;
        public readonly OracleConnection RelatedConn;
        private const int DefaultBatchNum = 2000;
        private DumpTask _task;

        public Importer(DumpTask dumpTask)
        {
            _task = dumpTask;
        }

        public int Import()
        {
            var ds=new DataColumnMapping();
            var batchNum = DefaultBatchNum;
            var result = 0;
            //定义批处理的
            var models = new object[_task.MappingList.Count][];
            SourceConn.Open();
            RelatedConn.Open();
            var sourceCmd = new SqlCommand
            {
                Connection = SourceConn,
                CommandText =_task.SourceSqlStr
            };
            var reader = sourceCmd.ExecuteReader();
            var relatedCmd = new OracleCommand
            {
                Connection = RelatedConn,
                CommandText = GetInsertCmdText()
            };
            var i = 0;
            try
            {
                while (reader.Read())
                {
                    var objs = new object[DefaultBatchNum];
                    
                    models[i] = objs;
                    i++;
                    if (i == batchNum)
                    {
                        CommitBatch(models, relatedCmd);
                        result += i;
                        i = 0;
                        models = new object[batchNum][];//重置批
                    }
                }
                if (i > 0)
                {
                    var oddModels = new object[batchNum][];
                    for (int j = 0; j < i; j++)
                    {
                        oddModels[j] = models[i - 1];
                    }
                    CommitBatch(oddModels, relatedCmd);
                    result += i;
                }
                reader.Close();
            }
            catch (Exception e)
            {
                log4net.LogManager.GetLogger("Logger").Error(e.Message + "\r\n" + sourceCmd.CommandText + "\r\n" + relatedCmd.CommandText);
                throw;
            }
            finally
            {
                SourceConn.Close();
                RelatedConn.Close();
            }
            return result;
        }

        protected virtual void CommitBatch(object[][] objects, OracleCommand relatedCommand)
        {
            var tx = RelatedConn.BeginTransaction();
            try
            {
                relatedCommand.Transaction = tx;
                relatedCommand.ArrayBindCount = modelList.Length;
                var propertyInfos = GetModelType().GetProperties().Where(x => x.Name != "MESTimeStamp").ToArray();
                var arry = new object[propertyInfos.Length][];
                for (var i = 0; i < arry.Length; i++)
                {
                    arry[i] = new object[BatchNum];
                }
                for (var i = 0; i < propertyInfos.Length; i++)
                {
                    for (var j = 0; j < modelList.Length; j++)
                    {
                        arry[i][j] = propertyInfos[i].GetValue(modelList[j]);
                    }
                }
                for (var i = 0; i < propertyInfos.Length; i++)
                {
                    relatedCommand.Parameters.Add(new OracleParameter(propertyInfos[i].Name, DbDataTypeMapper.GetOracleDataType(propertyInfos[i])) { Value = arry[i] });
                }

                relatedCommand.ExecuteNonQuery();
                tx.Commit();
                relatedCommand.Parameters.Clear();
            }
            catch (Exception)
            {
                tx.Rollback();
                log4net.LogManager.GetLogger("Logger").Error(relatedCommand.CommandText);
                throw;
            }
        }

        protected virtual string GetInsertCmdText()
        {
            var propInfos = type.GetProperties().Where(x => x.Name != "MESTimeStamp").ToArray();
            return
                $"insert into {type.Name} ({string.Join(",", propInfos.Select(x => x.Name))}) values({string.Join(",", propInfos.Select(x => ":" + x.Name))})";
        }
    }
}
