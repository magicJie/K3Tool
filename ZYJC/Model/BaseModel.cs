using System;
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
    }
}