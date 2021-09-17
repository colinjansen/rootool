using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Replication.RooTool.Core.Entities;

namespace Replication.RooTool.Data
{
    public class RooToolDbContext : DbContext
    {
        public RooToolDbContext(DbContextOptions<RooToolDbContext> options) : base(options)
        { }

        public DbSet<DataMapping> DataMappings { get; set; }
    }

    // this exists for building migrations from the command line
    // it requires an empty constructor but still needs some configuration
    // so we're using the development environment connection string
    public class RooToolDbContextFactory : IDesignTimeDbContextFactory<RooToolDbContext>
    {
        public RooToolDbContext CreateDbContext(string[] args)
        {
            var contextOptions = new DbContextOptionsBuilder<RooToolDbContext>();
            contextOptions.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=RooTool;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False", dbOptions => 
            {
                dbOptions.MigrationsHistoryTable("__migrations");
            });
            
            return new RooToolDbContext(contextOptions.Options);
        }
    }
}