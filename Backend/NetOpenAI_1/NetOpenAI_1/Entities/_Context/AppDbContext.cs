using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace NetOpenAI_1.Entities._Context
{
    public class AppDbContext : AppDbSet
    {
        private string nameOrConnectionString;
        private bool? lazyLoading;
        private bool? activeLog;

        public AppDbContext()
        {
            this.nameOrConnectionString = "DefaultConnection";
            this.lazyLoading = true;
            this.activeLog = false;
        }

        /// <summary>
        /// Configuración de la conexión
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Archivo base
                                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true); // Archivo específico del entorno
            var configuration = builder.Build();

            // Configuración predeterminada
            nameOrConnectionString ??= "DefaultConnection";
            lazyLoading ??= true;
            activeLog ??= false;

            if (lazyLoading.Value)
            {
                optionsBuilder.UseLazyLoadingProxies();
            }

            if (activeLog.Value || Debugger.IsAttached)
            {
                optionsBuilder.LogTo(message => Debug.WriteLine(message));
            }

            optionsBuilder.UseMySQL(
                configuration["ConnectionStrings:" + nameOrConnectionString],
                mysqlOptions => mysqlOptions.CommandTimeout(300)
            );
        }
    }
}
