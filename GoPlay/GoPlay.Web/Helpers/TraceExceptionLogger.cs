using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace GoPlay.Web.Helpers
{
    public class TraceExceptionLogger : ExceptionLogger
    {
        private static readonly ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void Log(ExceptionLoggerContext context)
        {
            string bodyText = string.Empty;
            using (var sr = new StreamReader(HttpContext.Current.Request.InputStream))
            {
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                bodyText = sr.ReadToEnd();
            }
            StringBuilder errorBuilder = new StringBuilder();
            errorBuilder.AppendLine("--------------------------: API ");
            errorBuilder.AppendLine("URL:  " + context.Request.RequestUri.AbsoluteUri);
            errorBuilder.AppendLine("Params:");
            errorBuilder.AppendLine(bodyText);
            errorBuilder.AppendLine(Environment.NewLine);
            errorBuilder.AppendLine(context.ExceptionContext.Exception.ToString());

            //Log file and send email
            logger.Error(errorBuilder.ToString());
        }
    }
}

