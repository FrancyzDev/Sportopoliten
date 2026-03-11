using Microsoft.EntityFrameworkCore;
using Sportopoliten.DAL.Data;

namespace Sportopoliten.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration["DatabaseProvider"];

            switch (provider)
            {
                case "Postgres":
                    services.AddDbContext<ShopDbContext>(options =>
                        options.UseNpgsql(
                            configuration.GetConnectionString("Postgres")));
                    break;
                case "MySql":
                    services.AddDbContext<ShopDbContext>(options =>
                        options.UseMySql(
                            configuration.GetConnectionString("MySql"),
                            ServerVersion.AutoDetect(
                                configuration.GetConnectionString("MySql"))));
                    break;
                case "SqlServer":
                    services.AddDbContext<ShopDbContext>(options =>
                        options.UseSqlServer(
                            configuration.GetConnectionString("SqlServer")));
                    break;
            }
            
            return services;
        }
    }
}
