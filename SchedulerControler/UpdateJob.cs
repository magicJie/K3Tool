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
                foreach (Source source in Configuration.Current.Sources)
                {
                    BaseImporter importer = new MaterielImporter(source);
                    //importer.SetLastUpdateTime();
                    logger.Info($"计划任务成功从{source.Name}新增(更新)物料 {importer.Update(new DateTime(1970, 1, 1), DateTime.Now)}条");
                    importer = new BOMImporter(source);
                    //importer.SetLastUpdateTime();
                    logger.Info($"计划任务成功从{source.Name}新增(更新)BOM {importer.Update(new DateTime(1970, 1, 1), DateTime.Now)}条");
                    importer = new ProductionPlanImporter(source);
                    //importer.SetLastUpdateTime();
                    logger.Info($"计划任务成功从{source.Name}新增(更新)生产计划 {importer.Update(new DateTime(1970, 1, 1), DateTime.Now)}条");
                }
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
