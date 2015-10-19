using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using GToken.Web.Identity;
using GToken.Web.MapperProfile;
using AutoMapper;
using AutoMapper.Mappers;
using System.Collections.Generic;
using Quartz;
using GToken.Identity;
using Quartz.Impl;

namespace GToken.Web
{
    public class DependancyInjectionConfig
    {
        public static void Setup()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            // Register for API Controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterFilterProvider();
            builder.RegisterModelBinderProvider();
            builder.RegisterModule<AutofacWebTypesModule>();

            // Find all autofac modules and include them.
            builder.RegisterModule<IdentityModule>();

            // Find all IStartable tasks and register them.
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //builder.RegisterAssemblyTypes(assemblies)
            //    .AssignableTo<IStartable>()
            //    .As<IStartable>()
            //    .SingleInstance();

            builder.RegisterType<EntityMappingProfile>().As<Profile>();

            builder.Register(ctx => new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers))
                .AsImplementedInterfaces()
                .SingleInstance()
                .OnActivating(x =>
                {
                    foreach (var profile in x.Context.Resolve<IEnumerable<Profile>>())
                    {
                        x.Instance.AddProfile(profile);
                    }
                });

            builder.RegisterType<MappingEngine>().As<IMappingEngine>();

            // Enable property injection into action filters (including authorize attribute).
            builder.RegisterFilterProvider();

            // Schedule jobs
            builder.Register(x => new StdSchedulerFactory().GetScheduler()).As<IScheduler>();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(x => typeof(IJob).IsAssignableFrom(x));

            IContainer container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));



            //Schedule
            IScheduler sched = container.Resolve<IScheduler>();
            sched.JobFactory = new AutofacJobFactory(container);
            sched.Start();

            IJobDetail job = JobBuilder.Create<JobScheduled>().Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("Scheduled to run Referral Campaigns")
                .WithCronSchedule(ConfigurationManager.AppSettings["SCHEDULED-CAMPAIGNS"])
                .StartNow()
                .Build();

            sched.ScheduleJob(job, trigger);
        }
    }
}