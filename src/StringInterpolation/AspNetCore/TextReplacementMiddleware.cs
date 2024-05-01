using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using StringInterpolation.Core;
using StringInterpolation.Core.Domain;

namespace StringInterpolation.AspNetCore
{
    public class TextReplacementMiddleware
    {
        private readonly RequestDelegate _next;

        public TextReplacementMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(InterpolationConfig.KeyName, out StringValues partner))
            {
                await _next(context);
                return;
            }

            using var textStream = new TextReplacementStream(context.Response.Body, partner);
            context.Response.Body = textStream;

            await _next(context);
        }
    }
}
