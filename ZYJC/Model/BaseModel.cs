using System;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ZYJC.Model
{
    public abstract class BaseModel
    {
        [DbType(DbDataType = "Char")]
        public char Flag { set; get; }
        [DbType(DbDataType = "TimeStamp")]
        public DateTime K3TimeStamp { set; get; }
        [DbType(DbDataType = "TimeStamp")]
        public DateTime MESTimeStamp { set; get; }
        public string SourceDb { set; get; }
        public string ID { set; get; }
        public string HashCode { set; get; }
        public void CalculateHashCode()
        {
            var md5Hasher = MD5.Create();
            var str= string.Join(",", GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(x => $"{ x.Name}:{x.GetValue(this, null)}"));
            var data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (var s in data)
            {
                sb.Append(s.ToString("x2"));
            }
            HashCode = sb.ToString();
        }
    }
}