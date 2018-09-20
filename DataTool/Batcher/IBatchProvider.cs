using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTool.Batcher
{
    interface IBatchProvider
    {
        DbConnection DbConnection { set; get; }
        void Insert(DataTable dt);
        void Insert(DataTable dt, string targetTableName, Dictionary<string, string> mapping);
        void Update(DataTable dt);
        void Update(DataTable dt, string targetTableName, Dictionary<string, string> mapping);
        void Delete(DataTable dt);
        void Delete(DataTable dt, string targetTableName);
    }
}
