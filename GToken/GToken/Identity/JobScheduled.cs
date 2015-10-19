using Autofac;
using Platform.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GToken.Identity
{
    public class JobScheduled : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //Get all referral campaigns to find items need to start or finish
            var api = Platform.Core.Api.Instance;
            DateTime now = DateTime.Now;
            DateTime timeCompare = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            List<int> status = new List<int>();
            status.Add((int)ReferralCampaignStatus.Active);
            status.Add((int)ReferralCampaignStatus.Running);

            var listCampaigns = api.GetReferralCampaigns(status).Data;
            //list campaigns will be updated to running
            var listActive = listCampaigns
                .Where(c =>
                       c.status == (int)ReferralCampaignStatus.Active
                    && c.is_display_only == false
                    && c.start_date <= timeCompare)
                .Select(c=>c.id).ToList();

            //list campaigns will be updated to finish
            var listRunning = listCampaigns
                .Where(c =>
                       c.status == (int)ReferralCampaignStatus.Running
                    && c.end_date <= timeCompare)
                .Select(c=>c.id).ToList();

            if (listActive != null && listActive.Any())
                api.UpdateReferralCampaigns(listActive, (int)ReferralCampaignStatus.Running);

            if (listRunning != null && listRunning.Any())
                api.UpdateReferralCampaigns(listRunning, (int)ReferralCampaignStatus.Finished);
        }
    }

    public class AutofacJobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public AutofacJobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_container.Resolve(bundle.JobDetail.JobType);
        }


        public void ReturnJob(IJob job)
        {
        }
    }
}