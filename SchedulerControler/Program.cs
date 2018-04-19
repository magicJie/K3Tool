using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZYJC
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
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
