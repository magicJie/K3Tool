﻿using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZYJC.Importer;

namespace ZYJC
{
    public class BackUpdateJob : IJob
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
                logger.Info($"标记删除物料{new MaterielImporter().BackUpdate(new DateTime(), DateTime.Now)}条");
                logger.Info($"标记删除BOM{new BOMImporter().BackUpdate(new DateTime(), DateTime.Now)}条");
                logger.Info($"标记删除生产计划{new ProductionPlanImporter().BackUpdate(new DateTime(), DateTime.Now)}条");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
            finally {
                Scheduler.Instance.SetExecuted(id);
            }
        }
    }
}