using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SaveUpBackend.Common;
using SaveUpBackend.Common.Extensions;
using SaveUpBackend.Common.Generics;
using SaveUpBackend.Data;
using SaveUpBackend.Interfaces;
using SaveUpBackend.Middleware;
using SaveUpBackend.Services;
using SaveUpModels.Common.Extensions;
using SaveUpModels.DTOs.Requests;
using SaveUpModels.Enums;

namespace SaveUpBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.SetupSerilog();
            builder.Services.Configure<RouteOptions>(options => 
            {
                options.LowercaseUrls = true;
            });

            builder.Services.AddAutoMapperProfile();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();

            //Service registrations
            builder.Services.AddScoped<IMongoDBContext, MongoDBContext>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IItemService, ItemService>();

            builder.Services.AddScoped(typeof(GenericService<,,,>));


            builder.Services.AddControllers();

            // builder.SetupCORS();
            builder.SetupAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

           // app.UseCors(AppDomain.CurrentDomain.FriendlyName);

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            InitializeDatabase(app).Wait();

            app.Run();
        }

        private static async Task InitializeDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IMongoDBContext>();
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                if(!await context.Users.AnyAsync())
                {
                    await userService.CreateAsync(new CreateUserRequest
                    {
                        Password = "admin",
                        Username = "admin",
                        Role = RoleNames.SuperAdmin
                    });
                    await userService.CreateAsync(new CreateUserRequest
                    {
                        Password = "user",
                        Username = "user",
                        Role = RoleNames.User
                    });
                }

                
            }
        }

    }
}
