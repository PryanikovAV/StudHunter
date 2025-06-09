using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StudHunter.DB.Postgres;

namespace StudHunter.Migrations;

internal class StudHunterContextFactory : IDesignTimeDbContextFactory<StudHunterDbContext>
{
    static StudHunterContextFactory()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public StudHunterDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<StudHunterDbContext>();
        
        optionsBuilder.UseNpgsql(
            connectionString,
            options => options
                .CommandTimeout(60)
                .MigrationsAssembly("StudHunter.Migrations")
                .MigrationsHistoryTable("__EFMigrationsHistory", "studhunter"));

        return new StudHunterDbContext(optionsBuilder.Options);
    }
}
