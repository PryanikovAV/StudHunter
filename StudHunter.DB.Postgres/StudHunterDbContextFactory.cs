using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace StudHunter.DB.Postgres;

public class StudHunterDbContextFactory : IDesignTimeDbContextFactory<StudHunterDbContext>
{
    public StudHunterDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<StudHunterDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new StudHunterDbContext(optionsBuilder.Options);
    }
}
