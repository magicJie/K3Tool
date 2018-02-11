using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace DataTool.Model
{
    [Serializable]
    public class Scheme
    {
        public string Name { set; get; }
        public string Author { set; get; }
        public DateTime LastModifiedTime { set; get; }
        public UpdateMode UpdateMode { set; get; }
        public DumpMode DumpMode { set; get; }
        public DeleteMode DeleteMode { set; get; }
        public int? BatchNum { set; get; }
        public DatabaseType SourceDbType { set; get; }
        public IPAddress SourceDbIP { set; get; }
        public int SourceDbPort { set; get; }
        public string SourceDbUserName { set; get; }
        public string SourceDbPassword { set; get; }
        public DatabaseType TargetDbType { set; get; }
        public IPAddress TargetDbIP { set; get; }
        public int TargetDbPort { set; get; }
        public string TargetDbUserName { set; get; }
        public string TargetDbPassword { set; get; }
        public List<DumpTask> TaskList { set; get; }

        /// <summary>
        /// 将方案持久化到xml文件
        /// </summary>
        public void Save(string fileName="")
        {
            if (fileName == "")
                fileName = string.Concat(Name, ".scheme");
            if (!fileName.ToLower().EndsWith(".scheme"))
                fileName = string.Concat(fileName, ".scheme");
            using (FileStream fs=new FileStream(fileName,FileMode.Create))
            {
                new XmlSerializer(GetType()).Serialize(fs,this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public static Scheme Load(string fileName)
        {
            using (FileStream fs=new FileStream(fileName,FileMode.Open))
            {
                return new XmlSerializer(typeof(Scheme)).Deserialize(fs) as Scheme;
            }
        }
    }

    public enum DatabaseType
    {
        Oracle,
        SqlServer
    }

    /// <summary>
    /// 更新模式
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// 根据主键，导入时强制覆盖已存在于目标表中的数据
        /// </summary>
        Override,
        /// <summary>
        /// 目标表要对比和源数据的修改时间，只有数据过期才会更新
        /// </summary>
        Update
    }

    /// <summary>
    /// 删除模式
    /// </summary>
    public enum DeleteMode
    {
        /// <summary>
        /// 目标表对比源表，如果源表没有对应行，标识为删除
        /// </summary>
        Flag,
        /// <summary>
        /// 目标表对比源表，如果源表没有对应行，删除目标表中数据
        /// </summary>
        Delete,
        /// <summary>
        /// 目标数据库不对源数据库进行反向对比
        /// </summary>
        Ignore
    }

    /// <summary>
    /// 导入模式
    /// <para>只有在指定UpdateMode为Update此项才能生效</para>
    /// </summary>
    public enum DumpMode
    {
        /// <summary>
        /// 数据初始化模式
        /// </summary>
        Init,
        /// <summary>
        /// 数据更新模式
        /// </summary>
        Update
    }
}
