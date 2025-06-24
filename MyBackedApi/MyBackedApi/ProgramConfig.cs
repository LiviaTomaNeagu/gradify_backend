using Infrastructure.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Text;

namespace MyBackedApi
{
    public static class ProgramConfig
    {
        public static void AddJsonSerializer(this IMvcBuilder builder)
        {
            builder.AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                x.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            }
            );
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend Api", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Just enter your token in the box below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        }, new List<string>()
                    }
                });
                options.AddServer(new OpenApiServer { Url = "/" });
            });
        }

        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = AppConfig.JwtSettings.Issuer,
                    ValidAudience = AppConfig.JwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConfig.JwtSettings.Secret))
                };
            });
        }

        public static void UseAppSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevEnv())
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "api/swagger/{documentname}/swagger.json";
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Backend Api");
                    c.RoutePrefix = "api/swagger";
                });
            }
        }

        public static bool IsDevEnv(this IWebHostEnvironment env)
        {
            return env.IsDevelopment() || env.EnvironmentName == "QA" || env.EnvironmentName == "Local";
        }
        public static bool IsNonLocalEnv(this IWebHostEnvironment env)
        {
            return env.EnvironmentName != "Local";
        }
    }

}
