using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            // Add Generic Database Context
            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("eCommerceConnection"),
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

            // Configure Serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.File(path: $"{fileName}-.text",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp: yyyy-MM-dd HH:mm:ss.fff zzz}[Level:u3]{message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Add JWT Authentication Shceme
            JWTAuthenticationScheme.AddJwtAuthenticationScheme(services, config);

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // Use global Exception
            app.UseMiddleware<GlobalException>();

            //Register middleware to block all outsiders API calls
            app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }
    }
}
