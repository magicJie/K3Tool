using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTool.Model
{
    [Serializable]
    public class DumpTask
    {
        public string Name { set; get; }
        public string SourceSqlStr { set; get; }
        public string TargetTableName { set; get; }
        public List<Mapping> MappingList { set; get; }
    }
}
