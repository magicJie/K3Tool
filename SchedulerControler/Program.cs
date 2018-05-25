using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ZYJC.Model;

namespace ZYJC
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.LogManager.GetLogger("logger").Info($@"准备启动计划任务");
            Configuration.Load();
            Scheduler.Instance.Start();
        }
    }
}
