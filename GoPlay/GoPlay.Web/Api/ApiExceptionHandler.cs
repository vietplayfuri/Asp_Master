using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace GoPlay.Web.Api
{
    public class ApiExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var statusCode = context.ExceptionContext.Response != null && context.ExceptionContext.Response.StatusCode != null ?
                context.ExceptionContext.Response.StatusCode : HttpStatusCode.BadRequest;
            var errorCode = (int)statusCode;
            var content = Json.Encode(new { error = 1, errorCode = errorCode, errorMessage = context.Exception.Message });
            context.Result = new ApiErrorActionResult(context.Request, content, statusCode);
            base.Handle(context);
        }
    }

    public class ApiErrorActionResult : IHttpActionResult
    {
        public HttpRequestMessage Request { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }

        public ApiErrorActionResult(HttpRequestMessage request, string content, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            Request = request;
            StatusCode = statusCode;
            Content = content;
        }
        public System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(Content);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response.RequestMessage = Request;
            return Task.FromResult(response);
        }
    }
}