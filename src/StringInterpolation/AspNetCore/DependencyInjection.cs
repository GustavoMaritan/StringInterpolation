using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace StringInterpolation.AspNetCore
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInterpolationFilter(this IServiceCollection service)
        {
            service.AddScoped<TextReplacementFilter>();

            return service;
        }

        public static IApplicationBuilder UseInterpolationMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<TextReplacementMiddleware>();

            return app;
        }

        public static WebApplication AddInterpolationFilter(this WebApplication app)
        {
            app.UseMiddleware<TextReplacementMiddleware>();

            return app;
        }
    }
}
