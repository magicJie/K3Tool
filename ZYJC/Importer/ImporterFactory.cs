using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZYJC.Importer
{
    /// <summary>
    /// 简单工厂模式：导入器工厂
    /// </summary>
    public class ImporterFactory
    {
        /// <summary>
        /// 根据导入器类名反射使用默认构造方法产生一个导入器
        /// </summary>
        /// <param name="importerName">导入器类名</param>
        /// <returns>默认构造方法产生的导入器</returns>
        public static BaseImporter NewImporter(string importerName)
        {

        }
    }
}
