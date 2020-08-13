using System.IO;
using JN.ApiDemo.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JN.ApiDemo.Identity
{
    public class DbContextFactory : IDesignTimeDbContextFactory<IdentityDataContext>
    {
        public IdentityDataContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder<IdentityDataContext>();

            var connectionString = configuration
                .GetConnectionString("SqlConnectionIdentity");

            dbContextBuilder.UseSqlServer(connectionString);

            var options = dbContextBuilder.Options;

            return new IdentityDataContext(options);
        }


    }
}
