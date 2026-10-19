using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.PostgresMigrations;

public sealed class PostgresDesignTimeFactory : IDesignTimeDbContextFactory<ProjectContext>
{
    public ProjectContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? "Host=localhost;Port=5432;Database=pompa;Username=pompa;Password=pompa";

        var options = new DbContextOptionsBuilder<ProjectContext>()
            .UseNpgsql(
                PostgresConnectionString.Normalize(connectionString),
                npgsql => npgsql.MigrationsAssembly(typeof(PostgresDesignTimeFactory).Assembly.FullName))
            .Options;

        return new ProjectContext(options);
    }
}
