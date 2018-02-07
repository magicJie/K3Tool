using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using ZYJC.Model;

namespace ZYJC.Importer
{
    public class BaseImporter
    {
        public readonly SqlConnection SourceConn = new SqlConnection(ConfigurationManager.ConnectionStrings["source"].ConnectionString);
        public readonly OracleConnection RelatedConn =new OracleConnection(ConfigurationManager.ConnectionStrings["related"].ConnectionString);
        public const int BatchNum = 2000;

        public virtual Type GetModeType()
        {
            return typeof(BaseModel);
        }

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

        protected virtual void CommitBatch(BaseModel[] modelList, OracleCommand relatedCommand)
        {
            var tx = RelatedConn.BeginTransaction();
            try
            {
                relatedCommand.Transaction = tx;
                relatedCommand.ArrayBindCount = modelList.Length;
                var propertyInfos = GetModeType().GetProperties().Where(x => x.Name != "MESTimeStamp").ToArray();
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
            var type = GetModeType();
            var propInfos = type.GetProperties().Where(x => x.Name != "MESTimeStamp").ToArray();
            return
                $"insert into {type.Name} ({string.Join(",",propInfos.Select(x => x.Name))}) values({string.Join(",",propInfos.Select(x => ":" + x.Name))})";
        }
    }
}