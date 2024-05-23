using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HangfireTest.Db;

public class TrackingDbContext : DbContext
{

    public static readonly string SCHEMA = "tracking";
    public static readonly string MIGRATIONS = "_migrations";

    public DbSet<HealthCheckEntry> HealthCheck { get; set; }

    private readonly string connection;

    public TrackingDbContext(IConfiguration configuration) {
        connection = configuration.GetConnectionString("PostgreSQL")
            ?? throw new Exception("postres configuration string is not found");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SCHEMA);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        if (!builder.IsConfigured)
        {
            builder.UseNpgsql(connection, x => x.MigrationsHistoryTable(MIGRATIONS, SCHEMA));
        }
        base.OnConfiguring(builder);
    }
}

[Table("health_check")]
public class HealthCheckEntry
{
    [Key]
    public required string serverId { get; set; }
    public required string url { get; set; }
}
