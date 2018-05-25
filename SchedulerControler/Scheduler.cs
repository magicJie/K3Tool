using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;

namespace ZYJC
{
    public class Scheduler
    {
        private static Scheduler _Instance;
        public static Scheduler Instance
        {
            get
            {
                return _Instance ?? new Scheduler();
            }
        }

        private IScheduler _scheduler;
        private readonly List<String> _excutingJob = new List<string>();
        private const string BackUpdateJobId = "BackUpdateJob";
        private const string UpdateJobId = "UpdateJob";

        protected Scheduler() { }

        public void Start()
        {
            if (_scheduler != null)
                return;
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.Start();
            _scheduler.DeleteJob(new JobKey(UpdateJobId));
            _scheduler.DeleteJob(new JobKey(BackUpdateJobId));

            var triggerBuilder = TriggerBuilder.Create().WithIdentity(UpdateJobId);
            triggerBuilder.WithSimpleSchedule(x => x.WithIntervalInHours(Convert.ToInt16(Configuration.Current.Cycle)).RepeatForever()).StartNow();
            var trigger = triggerBuilder.Build();
            triggerBuilder = TriggerBuilder.Create().WithIdentity(BackUpdateJobId);
            triggerBuilder.WithSimpleSchedule(x => x.WithIntervalInHours(Convert.ToInt16(Configuration.Current.BackCycle)).RepeatForever()).StartNow();
            var trigger1 = triggerBuilder.Build();

            var job = JobBuilder.Create<UpdateJob>().WithIdentity(UpdateJobId).Build();
            var job1 = JobBuilder.Create<BackUpdateJob>().WithIdentity(BackUpdateJobId).Build();
            _scheduler.ScheduleJob(job, trigger);
            _scheduler.ScheduleJob(job1, trigger1);
        }

        public void SetExecuting(String jobName)
        {
            _excutingJob.Add(jobName);
        }

        public void SetExecuted(String jobName)
        {
            _excutingJob.Remove(jobName);
        }

        public bool IsExecuting(String jobName)
        {
            return _excutingJob.Contains(jobName);
        }
    }
}
