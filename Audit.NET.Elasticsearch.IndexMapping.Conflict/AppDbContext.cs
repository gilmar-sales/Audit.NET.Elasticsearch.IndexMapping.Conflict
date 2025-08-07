using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Audit.NET.Elasticsearch.IndexMapping.Conflict;

[AuditDbContext(ReloadDatabaseValues = true)]
public class AppDbContext : DbContext
{
    public DbSet<Person> Persons { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}