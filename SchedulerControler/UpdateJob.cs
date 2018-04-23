using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZYJC.Importer;


namespace ZYJC
{
    public class UpdateJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var id = context.JobDetail.Key.Name;
            if (Scheduler.Instance.IsExecuting(id))
            {
                return;
            }
            var logger = log4net.LogManager.GetLogger("logger");

            try
            {
                Scheduler.Instance.SetExecuting(id);
                BaseImporter importer = new MaterielImporter();
                //importer.SetLastUpdateTime();
                logger.Info($"计划任务成功更新物料 {importer.Update(new DateTime(1970,1,1), DateTime.Now)}条");
                importer = new BOMImporter();
                //importer.SetLastUpdateTime();
                logger.Info($"计划任务成功更新BOM {importer.Update(new DateTime(1970, 1, 1), DateTime.Now)}条");
                importer = new ProductionPlanImporter();
                //importer.SetLastUpdateTime();
                logger.Info($"计划任务成功更新生产计划 {importer.Update(new DateTime(1970, 1, 1), DateTime.Now)}条");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            finally
            {
                Scheduler.Instance.SetExecuted(id);
            }
        }
    }
}
