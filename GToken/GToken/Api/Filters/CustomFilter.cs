using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform.Models;
using Platform.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GToken.Web.Api.Filters
{
    public class ExecutionMeterAttribute : ActionFilterAttribute
    {
        private Stopwatch stopWatch = new Stopwatch();
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            stopWatch.Reset();
            stopWatch.Start();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            stopWatch.Stop();
            var executionTime = stopWatch.ElapsedMilliseconds;

            var objectContent = actionExecutedContext.Response.Content as ObjectContent;

            if (objectContent != null)
            {
                JObject jobject = JsonHelper.DeserializeObject<JObject>(JsonConvert.SerializeObject(objectContent.Value));
                jobject["execution_time"] = executionTime;

                objectContent.Value = jobject;

                actionExecutedContext.ActionContext.Response.Content = objectContent;
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}