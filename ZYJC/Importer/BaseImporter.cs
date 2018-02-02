using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZYJC.Model;

namespace ZYJC.Importer
{
    public class BaseImporter
    {
        public readonly string SourceConnStr = ConfigurationManager.AppSettings["source"];
        public readonly string RelatedConnStr = ConfigurationManager.AppSettings["related"];
        private static BaseImporter _Instance;

        public static BaseImporter Instance
        {
            get
            {
                return _Instance == null ? new BaseImporter() : _Instance;
            }
        }

        protected BaseImporter()
        {
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <returns>导入行数</returns>
        public virtual int Import()
        {
            return 0;
        }
    }
}