using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using StringInterpolation.Core;
using StringInterpolation.Core.Domain;
using StringReplacement.Core;

namespace StringInterpolation.AspNetCore
{
    public class TextReplacementFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Request.Headers.TryGetValue(InterpolationConfig.KeyName, out StringValues partner);

            using var textStream = new TextReplacementStream(context.HttpContext.Response.Body, partner);
            context.HttpContext.Response.Body = textStream;

            await next();
        }
    }
}
