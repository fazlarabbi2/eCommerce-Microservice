using eCommerce.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace eCommerce.SharedLibrary.DependencyInjection
{
    public static class JWTAuthenticationScheme
    {
        public static IServiceCollection AddJwtAuthenticationScheme(this IServiceCollection services, IConfiguration config)
        {
            // Add JWT Service
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Bearer", options =>
            {
                var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);

                string issuer = config.GetSection("Authentication:Issuer").Value!;
                string audience = config.GetSection("Authentication:Audience").Value!;

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            return services;
        }


        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // User Global Exception
            app.UseMiddleware<GlobalException>();

            // Register middleware to block all outsiders API calls
            app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }
    }
}
