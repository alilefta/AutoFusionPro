using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace AutoFusionPro.Infrastructure.Data.Context
{

    public interface IApplicationDbContextFactory
    {
        ApplicationDbContext CreateDbContext();
    }

    public class ApplicationDbContextFactory : IApplicationDbContextFactory, IDesignTimeDbContextFactory<ApplicationDbContext>
    {

        public ApplicationDbContext CreateDbContext()
        {
            // Get the base directory of the application
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            Console.WriteLine($"The path for the database is = {baseDirectory}");

            var DbName = ConfigurationManager.AppSettings.Get("DbName") ?? "AutoFusionPro.db";
            Console.WriteLine($"The DbName is = {DbName}");


            // Build absolute path
            string dbPath = Path.Combine(baseDirectory, DbName);
            Console.WriteLine($"The DbPath is = {dbPath}");

            // Or use your hard-coded path similar to what you have in App.xaml.cs
            string connectionString = $"Data Source={dbPath}";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite(connectionString,
            b => b.MigrationsAssembly("AutoFusionPro.Infrastructure"));


            return new ApplicationDbContext(optionsBuilder.Options);
        }

        ApplicationDbContext IDesignTimeDbContextFactory<ApplicationDbContext>.CreateDbContext(string[] args)
        {
            return CreateDbContext();
        }
    }
}
