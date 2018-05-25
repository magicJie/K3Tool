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
        public SqlConnection SourceConn { get; }
        public Source Source { get; }
        public OracleConnection RelatedConn { get; } = new OracleConnection(ConfigurationManager.ConnectionStrings["related"].ConnectionString);
        public const int BatchNum = 2000;

        public virtual Type GetModelType()
        {
            return typeof(BaseModel);
        }

        public BaseImporter(Source dataSource)
        {
            SourceConn = new SqlConnection(dataSource.ConnectionString);
            Source = dataSource;
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>导入行数</returns>
        public virtual int InitImport(DateTime startTime, DateTime endTime)
        {
            return 0;
        }

        public virtual int UpdateImport(DateTime startTime, DateTime endTime)
        {
            return 0;
        }

        protected virtual void CommitBatch(OracleCommand relatedCommand, params BaseModel[] modelList)
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
                        arry[i][j] = propertyInfos[i].GetValue(modelList[j], null);
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
            catch (Exception ex)
            {
                tx.Rollback();
                log4net.LogManager.GetLogger("Logger").Error(ex.ToString() + relatedCommand.CommandText);
                throw;
            }
        }

        protected virtual string GetInsertCmdText()
        {
            var type = GetModelType();
            var propInfos = type.GetProperties().Where(x => x.Name != "MESTimeStamp").ToArray();
            return
                $"insert into {type.Name} ({string.Join(",", propInfos.Select(x => x.Name))}) values({string.Join(",", propInfos.Select(x => ":" + x.Name))})";
        }

        protected virtual string GetUpdateCmdText()
        {
            var type = GetModelType();
            var propInfos = type.GetProperties().Where(x => x.Name != "MESTimeStamp").ToArray();
            return $@"update {type.Name} set {string.Join(",", propInfos.Select(x => x.Name + "=" + ":" + x.Name))}";
        }

        protected virtual string GetDeleteCmdText() { throw new NotImplementedException(); }

        public virtual int BackUpdate(DateTime startTime, DateTime endTime) { throw new NotImplementedException(); }

        public virtual int Update(DateTime startTime, DateTime endTime) { throw new NotImplementedException(); }

        public virtual void Insert(BaseModel model)
        {
            RelatedConn.Open();
            try
            {
                var cmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = GetInsertCmdText()
                };
                CommitBatch(cmd, model);
            }
            catch
            {
                throw;
            }
            finally
            {
                RelatedConn.Close();
            }
        }

        public virtual void Update(BaseModel model)
        {
            RelatedConn.Open();
            try
            {
                var cmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = GetUpdateCmdText()
                };
                CommitBatch(cmd, model);
            }
            catch
            {
                throw;
            }
            finally
            {
                RelatedConn.Close();
            }
        }

        protected virtual void AddDeleteParameter(OracleCommand cmd, BaseModel model)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(BaseModel model)
        {
            RelatedConn.Open();
            try
            {
                var cmd = new OracleCommand
                {
                    Connection = RelatedConn,
                    CommandText = GetDeleteCmdText()
                };
                AddDeleteParameter(cmd, model);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("Logger").Error(ex.ToString());
                throw;
            }
            finally
            {
                RelatedConn.Close();
            }
        }

        public virtual DateTime GetLastUpdateTime()
        {
            throw new NotImplementedException();
        }

        public virtual void SetLastUpdateTime()
        {
            throw new NotImplementedException();
        }
    }
}