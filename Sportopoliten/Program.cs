using Microsoft.AspNetCore.Authentication.Cookies;
using Scalar.AspNetCore;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.BLL.Services;
using Sportopoliten.DAL.Interfaces;
using Sportopoliten.DAL.Repositories;
using Sportopoliten.Extensions;

namespace Sportopoliten
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddDatabase(builder.Configuration);

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.SlidingExpiration = true;
                });

            builder.Services.AddControllersWithViews();

            builder.Services.AddAuthorization();

            builder.Services.AddOpenApi("v1", options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new()
                    {
                        Title = "SPORTOPOLITEN API",
                        Version = "v1",
                        Description = "API для проекта SPORTOPOLITEN"
                    };
                    return Task.CompletedTask;
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options
                        .WithTitle("SPORTOPOLITEN API")
                        .WithTheme(ScalarTheme.Alternate)
                        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
                // Уберите эти строки или перенесите в нужное место
                // app.UseExceptionHandler("/Home/Error");
                // app.UseHsts();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();  // Добавьте эту строку для статических файлов
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}