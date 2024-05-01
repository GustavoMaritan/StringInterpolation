using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using StringInterpolation.Core.Domain;
using StringInterpolation.Core.Abstract;
using StringInterpolation.Core.Services;

namespace StringInterpolation.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInterpolationService(this IServiceCollection services)
        {
            services.AddScoped<ITextReplacementService, TextReplacementService>();
            return services;
        }

        public static IServiceCollection AddInterpolationProvider<T>(this IServiceCollection services) where T : IInterpolationProvider
        {
            var instance = (IInterpolationProvider)Activator.CreateInstance(typeof(T));
            services.AddTransient(x => instance);
            return services;
        }

        public static IServiceCollection AddInterpolation(this IServiceCollection services, Action<InterpolationOptions> options)
        {
            options(new InterpolationOptions());
            if (InterpolationConfig.Provider != null)
            {
                var instance = (IInterpolationProvider)Activator.CreateInstance(InterpolationConfig.Provider);
                services.AddTransient(x => instance);
            }

            return services;
        }

        public static WebApplication UseInterpolation(this WebApplication app)
        {
            LoadProvider(app.Services);
            return app;
        }

        public static IApplicationBuilder UseInterpolation(this IApplicationBuilder app)
        {
            LoadProvider(app.ApplicationServices);
            return app;
        }

        private static void LoadProvider(IServiceProvider provider)
        {
            var providerReplace = provider.GetRequiredService<IInterpolationProvider>()
                ?? throw new Exception("IInterpolationProvider deve ser implementado.");

            var lazyResult = new Lazy<Task<ConcurrentDictionary<string, Dictionary<string, string>>>>(
                async () => await providerReplace.LoadAsync()
             );

            StorageValues.Load(lazyResult);
        }
    }
}
