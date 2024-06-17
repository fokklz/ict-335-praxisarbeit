using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using SaveUpBackend.Common.Serilog;
using SaveUpBackend.Common.Swagger;
using Serilog;
using StackExchange.Redis;

namespace SaveUpBackend.Common.Extensions
{
    public static class WebApplicationBuilderExtension
    {
        /// <summary>
        /// Setup Serilog to work with the Application
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to Create a ASP.NET Web API</param>
        /// <returns>a <see cref="WebApplicationBuilder"> to build a ASP.NET Web Application</returns>
        public static WebApplicationBuilder SetupSerilog(this WebApplicationBuilder builder)
        {
            var redisConfig = builder.Configuration.GetSection("Databases").GetSection("Redis");
            var redisURL = redisConfig.GetValue("URL", "localhost")!;
            var redisName = redisConfig.GetValue("Name", "saveup-api");

            var LoggerFromSettings = new LoggerConfiguration()
                .WriteTo.Console();

            try
            {
                LoggerFromSettings = LoggerFromSettings.WriteTo.Redis(ConnectionMultiplexer.Connect(redisURL), $"{redisName}-logs");
            }
            catch
            {
                LoggerFromSettings = LoggerFromSettings.WriteTo.File($"../Logs/{redisName}-.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day);
            }

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(LoggerFromSettings.CreateLogger());

            return builder;
        }

        /// <summary>
        /// Add Authorization to Swagger and to the Application itself
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to Create a ASP.NET Web API</param>
        /// <returns>a <see cref="WebApplicationBuilder"> to build a ASP.NET Web Application</returns>
        public static WebApplicationBuilder SetupAuthorization(this WebApplicationBuilder builder)
        {
            var swaggerConfig = builder.Configuration.GetSection("Swagger");

            var version = swaggerConfig.GetValue("Version", "v1");
            var title = swaggerConfig.GetValue("Title", "SaveUp API");

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                c.OperationFilter<GenericResponseOperationFilter>();
                c.DocumentFilter<SortPathsByLengthDocumentFilter>();
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = builder.Configuration.GetTokenValidationParameters();
                });

            return builder;
        }

        /// <summary>
        /// Configure CORS inferred from the <see cref="IConfiguration"/> of the Application, will allow everything by default (methods, headers)
        /// </summary>
        /// <param name="builder">The WebApplicationBuilder used to Create a ASP.NET Web API</param>
        /// <returns>a <see cref="WebApplicationBuilder"> to build a ASP.NET Web Application</returns>
        public static WebApplicationBuilder SetupCORS(this WebApplicationBuilder builder)
        {
            var corsConfig = builder.Configuration.GetSection("CORS");

            var allowedOrigins = corsConfig["AllowedOrigins"]?.Split(',') ?? new string[0];
            var allowedMethods = corsConfig["AllowedMethods"]?.Split(',') ?? ["GET", "POST", "PUT", "DELETE", "OPTIONS"];
            var allowedHeaders = corsConfig["AllowedHeaders"]?.Split(',') ?? ["*"];
            var allowCredentials = bool.TryParse(corsConfig["AllowCredentials"], out var credentials) && credentials;

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(AppDomain.CurrentDomain.FriendlyName, policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .WithMethods(allowedMethods)
                          .WithHeaders(allowedHeaders);

                    if (allowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                    else
                    {
                        policy.DisallowCredentials();
                    }
                });
            });

            return builder;
        }

    }
}
