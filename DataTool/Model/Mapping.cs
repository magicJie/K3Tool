using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataTool.Model
{
    [Serializable]
    public class Mapping
    {
        [XmlIgnore]
        public DataColumn TargetColumn { set; get; }
        public string DataColumnName { set; get; }
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

        public DumpTask DumpTask
        {
            get
            {
                throw new System.NotImplementedException();
            }

            set
            {
            }
        }

        /// <summary>
        /// 根据映射信息返回映射值
        /// </summary>
        /// <param name="targetReader">源数据读取器</param>
        /// <returns>映射值（根据表达式运算）</returns>
        public object GetMappingValue(DbDataReader targetReader)
        {

            return "";
        }
    }
}
