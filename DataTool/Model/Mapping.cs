using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTool.Model
{
    [Serializable]
    public class Mapping
    {
        public string TargetField { set; get; }
        public string Expression { set; get; }
        public string DbDataType { set; get; }
        public bool IsRowState { set; get; }
        public bool IsTimeStamp { set; get; }
        public bool IsKeyField { set; get; }
    }
}
