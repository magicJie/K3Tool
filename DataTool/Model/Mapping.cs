using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTool.Model
{
    [Serializable]
    public class Mapping
    {
        public DataColumn TargetColumn { set; get; }
        public string Expression { set; get; }
        /// <summary>
        /// <para>当<paramref name="DataTool.Model.UpdateMode"/>设置为Update时，此属性才允许设置为true。</para>
        /// <para>设置为true的列，会用来表示行状态为创建、更新或者删除</para>
        /// </summary>
        public bool IsRowState { set; get; }
        /// <summary>
        /// <para>当<paramref name="DataTool.Model.UpdateMode"/>设置为Update时，此属性才允许设置为true。</para>
        /// <para>设置为true的列，会用来表示行的更新时间</para>
        /// </summary>
        public bool IsTimeStamp { set; get; }
    }
}
