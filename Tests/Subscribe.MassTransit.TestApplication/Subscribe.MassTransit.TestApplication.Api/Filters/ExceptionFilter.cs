using System;
using System.Diagnostics;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Subscribe.MassTransit.TestApplication.Domain.Common.Exceptions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.ExceptionFilter", Version = "1.0")]

namespace Subscribe.MassTransit.TestApplication.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case NotFoundException exception:
                    context.Result = new NotFoundObjectResult(new ProblemDetails
                    {
                        Detail = exception.Message
                    })
                    .AddContextInformation(context);
                    break;
            }
        }
    }

    internal static class ProblemDetailsExtensions
    {
        public static IActionResult AddContextInformation(this ObjectResult objectResult, ExceptionContext context)
        {
            if (objectResult.Value is not ProblemDetails problemDetails)
            {
                return objectResult;
            }
            problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
            problemDetails.Type = "https://httpstatuses.io/" + objectResult.StatusCode;
            return objectResult;
        }
    }
}