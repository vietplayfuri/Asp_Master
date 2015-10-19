using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using GoEat.Web.Identity;

namespace GoEat.Web
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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IStartable>()
                .As<IStartable>()
                .SingleInstance();

            // Enable property injection into action filters (including authorize attribute).
            builder.RegisterFilterProvider();

            IContainer container = builder.Build();
            var resolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}