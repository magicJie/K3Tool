using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ZYJC
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                log4net.LogManager.GetLogger("logger").Info($@"准备启动计划任务，当前配置为SourceDb【{ConfigurationManager.AppSettings["SourceDB"]}】；sourceConn:【{ConfigurationManager.ConnectionStrings["source"]}】；relatedConn:【{ConfigurationManager.ConnectionStrings["related"]}】");
                Scheduler.Instance.Start();
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("logger").Fatal(ex.ToString());
                throw;
            }
        }
    }
}
