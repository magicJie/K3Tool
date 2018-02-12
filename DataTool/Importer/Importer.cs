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
        public readonly DbConnection _sourceConn;
        public readonly DbConnection _targetConn;
        private readonly DbCommand _sourceCmd;
        private readonly DbCommand _targetCmd;
        private const int DefaultBatchNum = 2000;
        private readonly int _batchNum = DefaultBatchNum;
        private DumpTask _task;
        private Scheme _scheme;

        public Importer(DumpTask dumpTask)
        {
            _task = dumpTask;
            _scheme = dumpTask.Scheme;
            _sourceConn = GetSourceConn();
            _targetConn = GetTargetConn();
            _sourceCmd = GetSourceCmd();
            _targetCmd = GetTargetCmd();
            _sourceCmd.Connection = _sourceConn;
            _targetCmd.Connection = _targetConn;
            if (_scheme.BatchNum.HasValue)
                _batchNum = _scheme.BatchNum.Value;
        }

        public int Import()
        {
            var result = 0;
            //定义批处理的
            var models = new object[_task.MappingList.Count][];
            _sourceConn.Open();
            _targetConn.Open();
            //反向对比数据
            if (_scheme.DeleteMode != DeleteMode.Ignore)
            {
                BackUpdate();
            }

            var i = 0;
            try
            {
                while (sourceCmd.Read())
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
                log4net.LogManager.GetLogger("Logger").Error(e.Message + "\r\n" + _sourceCmd.CommandText + "\r\n" + _targetCmd.CommandText);
                throw;
            }
            finally
            {
                _sourceConn.Close();
                _targetConn.Close();
            }
            return result;
        }

        /// <summary>
        /// 反向更新
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private int BackUpdate()
        {
            var result = 0;
            if (_scheme.DeleteMode == DeleteMode.Flag)
            {
                _targetCmd.CommandText = $@"update {_task.TargetTableName} set xxx=xxx where xxx=xxx";
            }
            else if(_scheme.DeleteMode==DeleteMode.Delete)
            {
                _targetCmd.CommandText = $@"delete from  {_task.TargetTableName} where xxx=xxx";
            }
            var readCmd = GetTargetCmd();
            readCmd.Connection = _targetConn;
            readCmd.CommandText = $@"select {string.Join(",",_task.PrimaryKey.Select(x=>x.ColumnName))} from {_task.TargetTableName}";
            var reader = readCmd.ExecuteReader();
            var i = 0;
            var models=new object[_task.PrimaryKey.Length][];
            for (int j = 0; j < models.Length; j++)
            {
                models[j]=new object[_batchNum];
            }
            while (reader.Read())
            {
                _sourceCmd.CommandText = $@"select 0 from xxx where xxx in({string.Join(",", reader)})";
                var obj = _sourceCmd.ExecuteScalar();
                if (obj == DBNull.Value)
                {
                    var objs = new object[_batchNum];
                    for (int j = 0; j < models.Length; j++)
                    {
                        models[j][i] = reader[j];
                    }
                    models[i] = objs;
                    i++;
                }
                
                if (i == _batchNum)
                {
                    CommitBatch(models, _targetCmd);
                    result += i;
                    i = 0;
                    models = new object[_task.PrimaryKey.Length][];//重置批
                }
            }
            if (i > 0)
            {
                var oddModels = new object[_task.PrimaryKey.Length][];
                for (int j = 0; j < i; j++)
                {
                    oddModels[j] = models[j];
                }
                CommitBatch(oddModels, _targetCmd);
                result += i;
            }
            reader.Close();
            return result;
        }

        protected virtual void CommitBatch(object[][] objects, DbCommand cmd)
        {
            var tx = _sourceConn.BeginTransaction();
            try
            {
                cmd.Transaction = tx;
                for (var i = 0; i < objects.Length; i++)
                {
                    DbParameter para = GetDbParameter();
                    para.Value = objects[i];
                    cmd.Parameters.Add(para);
                }

                cmd.ExecuteNonQuery();
                tx.Commit();
                cmd.Parameters.Clear();
            }
            catch (Exception)
            {
                tx.Rollback();
                log4net.LogManager.GetLogger("Logger").Error(cmd.CommandText);
                throw;
            }
        }

        protected virtual string GetInsertCmdText()
        {
            var propInfos = type.GetProperties().Where(x => x.Name != "MESTimeStamp").ToArray();
            return
                $"insert into {type.Name} ({string.Join(",", propInfos.Select(x => x.Name))}) values({string.Join(",", propInfos.Select(x => ":" + x.Name))})";
        }

        private DbCommand GetSourceCmd()
        {
            if(_scheme.SourceDbType==DatabaseType.SqlServer)
                return new SqlCommand();
            else if(_scheme.SourceDbType == DatabaseType.Oracle)
                return new OracleCommand();
            throw new Exception("不支持的数据库"+_scheme.SourceDbType.ToString());
        }

        private DbCommand GetTargetCmd()
        {
            if (_scheme.TargetDbType == DatabaseType.SqlServer)
                return new SqlCommand();
            else if (_scheme.TargetDbType == DatabaseType.Oracle)
                return new OracleCommand();
            throw new Exception("不支持的数据库" + _scheme.TargetDbType.ToString());
        }

        private DbConnection GetSourceConn()
        {
            if (_scheme.SourceDbType == DatabaseType.SqlServer)
                return new SqlConnection();
            else if (_scheme.SourceDbType == DatabaseType.Oracle)
                return new OracleConnection();
            throw new Exception("不支持的数据库" + _scheme.SourceDbType.ToString());
        }

        private DbConnection GetTargetConn()
        {
            if (_scheme.TargetDbType == DatabaseType.SqlServer)
                return new SqlConnection();
            else if (_scheme.TargetDbType == DatabaseType.Oracle)
                return new OracleConnection();
            throw new Exception("不支持的数据库" + _scheme.TargetDbType.ToString());
        }

        
    }
}
