using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace ZYJC
{
    public class DbTypeAttribute:Attribute
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DbDataType { set; get; }
    }
}
