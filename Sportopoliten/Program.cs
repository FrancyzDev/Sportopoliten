using Scalar.AspNetCore;
using Sportopoliten.BLL.DTO;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.BLL.Services;
using Sportopoliten.DAL.Interfaces;
using Sportopoliten.DAL.Repositories;
using Sportopoliten.Extensions;
using System.Runtime.InteropServices;

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

            builder.Services.AddControllersWithViews();

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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Catalog}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}