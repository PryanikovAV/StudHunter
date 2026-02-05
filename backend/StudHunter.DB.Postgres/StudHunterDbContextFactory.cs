using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace StudHunter.DB.Postgres;

public class StudHunterDbContextFactory : IDesignTimeDbContextFactory<StudHunterDbContext>
{
    public StudHunterDbContext CreateDbContext(string[] args)
    {
        var apiPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "StudHunter.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<StudHunterDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new StudHunterDbContext(optionsBuilder.Options);
    }
}
