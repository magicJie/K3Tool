using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using DataTool.Batcher;
using DataTool.Model;

namespace DataTool.Importer
{
    public class Importer
    {
        private readonly DumpTask _task;
        private readonly Scheme _scheme;
        private readonly DbProviderFactory _sourceFactory;//抽象工厂模式一套代码支持多种数据库
        private readonly DbProviderFactory _targeFactory;
        private readonly DbConnection _sourceConn;
        private readonly DbConnection _targetConn;
        private const int DefaultBatchNum = 2000;
        private readonly int _batchNum = DefaultBatchNum;

        public Importer(DumpTask dumpTask)
        {
            _task = dumpTask;
            _scheme = dumpTask.Scheme;
            _sourceFactory = GetFactory(_scheme.SourceDbType);
            _targeFactory = GetFactory(_scheme.TargetDbType);
            _sourceConn = _sourceFactory.CreateConnection();
            _sourceConn.ConnectionString = _scheme.SourceConnStr;
            _targetConn = _targeFactory.CreateConnection();
            _targetConn.ConnectionString = _scheme.TargetConnStr;
            if (_scheme.BatchNum.HasValue)
                _batchNum = _scheme.BatchNum.Value;
        }

        public void Import()
        {
            //反向对比数据
            if (_scheme.DeleteMode != DeleteMode.Ignore)
            {
                BackUpdate();
            }
            Update();
        }

        private int Update()
        {
            var result = 0;
            var batcher = GetBatcher(_scheme.TargetDbType);
            var readCmd = _sourceConn.CreateCommand();
            readCmd.Connection = _sourceConn;
            readCmd.CommandText = _task.SourceSqlStr;
            var reader = readCmd.ExecuteReader();
            var targetQueryCmd = _targeFactory.CreateCommand();
            targetQueryCmd.Connection = _targetConn;
            var a = 0;//对新增批计数
            var b = 0;//对更新批计数
            var insertDt = new DataTable(_task.TargetTableName);//新增数据
            var updateDt = new DataTable(_task.TargetTableName);//更新数据
            _task.MappingList.ForEach(x =>
            {
                insertDt.Columns.Add(x.TargetColumn);
                updateDt.Columns.Add(x.TargetColumn);
            });
            //构造查询文本以及参数
            var whereStr = "";
            for (int j = 0; j < _task.PrimaryKey.Length; j++)
            {
                var para = targetQueryCmd.CreateParameter();
                para.ParameterName = _task.PrimaryKey[j].ColumnName;
                targetQueryCmd.Parameters.Add(para);
                whereStr += $@"{_task.PrimaryKey[j].ColumnName}={GetDbParameterChar(_scheme.SourceDbType)}{_task.PrimaryKey[j].ColumnName}";
                if (j != reader.FieldCount - 1)
                    whereStr += " and ";
            }
            targetQueryCmd.CommandText = $@"select 0 from {_task.TargetTableName} where {whereStr}";
            targetQueryCmd.Prepare();
            while (reader.Read())
            {
                for (int j = 0; j < reader.FieldCount; j++)
                {
                    targetQueryCmd.Parameters[j].Value = reader[j];
                }
                var obj = targetQueryCmd.ExecuteScalar();
                var objs = new List<object>();
                for (int j = 0; j < reader.FieldCount; j++)
                {
                    objs.Add(reader[j]);//TODO 采用Mapping转换数值
                }
                if (obj == DBNull.Value)
                {
                    insertDt.Rows.Add(objs.ToArray());
                    a++;
                }
                else
                {
                    updateDt.Rows.Add(objs.ToArray());
                    b++;
                }

                if (a == _batchNum)
                {
                    batcher.Insert(insertDt);
                    insertDt.Rows.Clear();
                    result += a;
                    a = 0;
                }
                if (b == _batchNum)
                {
                    batcher.Update(updateDt);
                    updateDt.Rows.Clear();
                    result += b;
                    b = 0;
                }
            }
            if (a > 0)
            {
                batcher.Insert(insertDt);
                result += a;
                a = 0;
            }
            if (b > 0)
            {
                batcher.Update(updateDt);
                result += b;
                b = 0;
            }
            reader.Close();
            return result;
        }

        /// <summary>
        /// 反向更新
        /// </summary>
        /// <returns></returns>
        private int BackUpdate()
        {
            var result = 0;
            var sourceCmd = _sourceFactory.CreateCommand();
            sourceCmd.Connection = _sourceConn;
            var batcher = GetBatcher(_scheme.TargetDbType);
            var readCmd = _targeFactory.CreateCommand();
            readCmd.Connection = _targetConn;
            readCmd.CommandText = $@"select {string.Join(",", _task.PrimaryKey.Select(x => x.ColumnName))} from {_task.TargetTableName}";
            var reader = readCmd.ExecuteReader();
            var i = 0;
            //构造表结构，区分删除模式
            var dt = new DataTable(_task.TargetTableName);
            dt.Columns.AddRange(_task.PrimaryKey);
            if (_scheme.DeleteMode == DeleteMode.Flag)
            {
                dt.Columns.Add(_task.MappingList.Find(x => x.IsRowState).TargetColumn);
            }
            var whereStr = "";
            for (int j = 0; j < reader.FieldCount; j++)
            {
                var para = sourceCmd.CreateParameter();
                para.ParameterName = _task.PrimaryKey[j].ColumnName;
                sourceCmd.Parameters.Add(para);
                whereStr += $@"{_task.PrimaryKey[j].ColumnName}={GetDbParameterChar(_scheme.SourceDbType)}{reader[j]}";
                if (j != reader.FieldCount - 1)
                    whereStr += " and ";
            }
            sourceCmd.CommandText = $@"select 0 from {_task.SourceTableName} where {whereStr}";
            sourceCmd.Prepare();
            while (reader.Read())
            {
                for (int j = 0; j < reader.FieldCount; j++)
                {
                    sourceCmd.Parameters[j].Value = reader[j];
                }
                var obj = sourceCmd.ExecuteScalar();
                if (obj == DBNull.Value)
                {
                    var objs = new List<object>();
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        objs.Add(reader[j]);
                    }
                    if (_scheme.DeleteMode == DeleteMode.Flag)
                    {
                        objs.Add(_task.MappingList.Find(x => x.IsRowState).GetMappingValue(reader));
                    }
                    dt.Rows.Add(objs.ToArray());
                    i++;
                }

                if (i == _batchNum)
                {
                    BackUpdateCommitBatch(batcher, dt);
                    result += i;
                    i = 0;
                }
            }
            if (i > 0)
            {
                BackUpdateCommitBatch(batcher, dt);
                result += i;
                i = 0;
            }
            reader.Close();
            return result;
        }

        private void BackUpdateCommitBatch(IBatchProvider batcher, DataTable dt)
        {
            if (_scheme.DeleteMode == DeleteMode.Flag)
            {
                batcher.Update(dt);
            }
            else if (_scheme.DeleteMode == DeleteMode.Delete)
            {
                batcher.Delete(dt);
            }
            dt.Rows.Clear();
        }

        private IBatchProvider GetBatcher(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.Oracle:
                    return new OracleBatcher();
                case DatabaseType.SqlServer:
                    return new SqlBatcher();
            }
            throw new Exception($@"未指定{databaseType}的ClientFactory");
        }

        private DbProviderFactory GetFactory(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.Oracle:
                    return DbProviderFactories.GetFactory("OracleClientFactory");
                case DatabaseType.SqlServer:
                    return DbProviderFactories.GetFactory("SqlClientFactory");
            }
            throw new Exception($@"未指定{databaseType}的ClientFactory");
        }

        private char GetDbParameterChar(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.Oracle:
                    return ':';
                case DatabaseType.SqlServer:
                    return '@';
            }
            throw new Exception($@"不支持的数据库{databaseType}");
        }
    }
}
