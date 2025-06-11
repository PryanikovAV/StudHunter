using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using StudHunter.DB.Postgres.Models;

AppContext.SetSwitch("Npgsql.Enable Legacy TimestampBehavior", true);

const string connectionString = "Host=localhost;Port=5433;"
                              + "Database=studhunter;"
                              + "Username=postgres;"
                              + "Password=postgres";

var optionsBuilder = new DbContextOptionsBuilder<StudHunterDbContext>();
optionsBuilder.UseNpgsql(connectionString);

using var db = new StudHunterDbContext(optionsBuilder.Options);

// db.Database.EnsureDeleted();
// db.Database.EnsureCreated();
