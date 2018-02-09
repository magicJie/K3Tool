using System;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace ZYJC
{
    public class DbDataTypeMapper
    {
        public static OracleDbType GetOracleDataType(PropertyInfo propInfo)
        {
            var objAttrs = propInfo.GetCustomAttributes(typeof(DbTypeAttribute), true);
            if (objAttrs.Length > 0)
            {
                var attr = (DbTypeAttribute)objAttrs[0];
                OracleDbType result;
                Enum.TryParse(attr.DbDataType, out result);
                return result;
            }

            //如果没有特性申明映射，则采用默认规则
            var type = propInfo.PropertyType;
            if (type.FullName == "System.String")
                return OracleDbType.NVarchar2;
            if(type.FullName=="System.DateTime")
                return OracleDbType.Date;
            if (type.FullName == "System.Double")
                return OracleDbType.Double;
            if (type.FullName == "System.Int16")
                return OracleDbType.Int16;
            if (type.FullName == "System.Int32")
                return OracleDbType.Int32;
            if (type.FullName == "System.Int64")
                return OracleDbType.Int64;
            if (type.FullName == "System.Decimal")
                return OracleDbType.Decimal;
            if (type.FullName == "System.Boolean")
                return OracleDbType.Char;
            throw new ArgumentOutOfRangeException(@"未能识别的数据类型"+type.FullName);
        }

        /// <summary>
        /// 将
        /// </summary>
        /// <param name="o1"></param>
        /// <param name="o2"></param>
        public static void SetValue(object o1,out object o2)
        {
            var type = o1.GetType();
            object a = null;
            if (o1.)
            {

            }
            if (type.FullName == "System.String")
            {
                o2 = (string) o1;
            }
            throw new Exception("数据类型不匹配或不支持的数据类型");
        }
    }
}
