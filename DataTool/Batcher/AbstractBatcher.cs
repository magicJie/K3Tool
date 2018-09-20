using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DataTool.Batcher
{
    public abstract class AbstractBatcher : IBatchProvider
    {
        public event EventHandler<BatcherAfterCommitEventArgs> AfterInsert;
        public event EventHandler<BatcherAfterCommitEventArgs> AfterUpdate;
        public event EventHandler<BatcherAfterCommitEventArgs> AfterDelete;

        private DbConnection _dbConnection;

        public virtual DbConnection DbConnection
        {
            get { return _dbConnection; }

            set { _dbConnection = value; }
        }

        public virtual void Insert(DataTable dt)
        {
            var mapping = new Dictionary<string, string>();
            foreach (DataColumn column in dt.Columns)
            {
                mapping.Add(column.ColumnName, column.ColumnName);
            }
            Insert(dt, dt.TableName, mapping);
        }

        public virtual void Insert(DataTable dt, string targetTableName, Dictionary<string, string> mapping)
        {
            AfterInsert?.Invoke(this, new BatcherAfterCommitEventArgs(dt, targetTableName, mapping));
        }

        public virtual void Update(DataTable dt)
        {
            var mapping = new Dictionary<string, string>();
            foreach (DataColumn column in dt.Columns)
            {
                mapping.Add(column.ColumnName, column.ColumnName);
            }
            Update(dt, dt.TableName, mapping);
        }

        public virtual void Update(DataTable dt, string targetTableName, Dictionary<string, string> mapping)
        {
            AfterUpdate?.Invoke(this, new BatcherAfterCommitEventArgs(dt, targetTableName, mapping));
        }

        public virtual void Delete(DataTable dt)
        {
            Delete(dt, dt.TableName);
        }

        public virtual void Delete(DataTable dt, string targetTableName)
        {
            AfterDelete?.Invoke(this, new BatcherAfterCommitEventArgs(dt, targetTableName));
        }

        public class BatcherAfterCommitEventArgs:EventArgs
        {
            public BatcherAfterCommitEventArgs()
            {
            }

            public BatcherAfterCommitEventArgs(DataTable dt, string targetTableName)
            {
                DataTable = dt;
                TargetTableName = targetTableName;
            }

            public BatcherAfterCommitEventArgs(DataTable dt, string targetTableName, Dictionary<string, string> mapping)
            {
                DataTable = dt;
                TargetTableName = targetTableName;
                Mapping = mapping;
            }

            public DataTable DataTable { set; get; }
            public string TargetTableName { set; get; }
            public Dictionary<string, string> Mapping { set; get; }
        }
    }
}
