using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataTool.Model
{
    [Serializable]
    public class DumpTask
    {
        public Scheme Scheme { get; }
        public string Name { set; get; }
        public string SourceSqlStr { set; get; }
        public string SourceTableName { set; get; }
        public string TargetTableName { set; get; }
        public List<Mapping> MappingList { set; get; } = new List<Mapping>();
        [XmlIgnore]
        public DataColumn[] PrimaryKey { set; get; }
        public string[] PrimaryKeys { set; get; }

        public DumpTask() { }

        public DumpTask(Scheme scheme)
        {
            Scheme = scheme;
            if(!scheme.TaskList.Contains(this))
                scheme.TaskList.Add(this);
            var ds=new DataColumn();
        }
    }
}
