using GoPlay.Web.Helpers;
using GoPlay.WebApi;
using Microsoft.AspNet.WebApi.MessageHandlers.Compression;
using Microsoft.AspNet.WebApi.MessageHandlers.Compression.Compressors;
using MultipartDataMediaFormatter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;

namespace GoPlay.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            /*
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            */

            // Register default route
            config.Routes.MapHttpRoute(
              name: "DefaultApiAction",
              routeTemplate: "api/{namespace}/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional }
             );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{namespace}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Services.Replace(typeof(IHttpControllerSelector), new NamespaceHttpControllerSelector(config));


            // Add compression message handler
            config.MessageHandlers.Insert(0, new ServerCompressionHandler(0, new GZipCompressor(), new DeflateCompressor()));

            // Configure error details policy
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            config.EnsureInitialized();
            config.Services.Add(typeof(IExceptionLogger), new TraceExceptionLogger());
            config.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter());
        }
    }
}
