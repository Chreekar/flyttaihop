using Microsoft.EntityFrameworkCore;

namespace Flyttaihop.Framework.Database
{
    public class CriteriaContext : DbContext
    {
        public CriteriaContext(DbContextOptions<CriteriaContext> options) : base(options) { }

        public DbSet<Models.Criteria> Criterias { get; set; }
        public DbSet<Models.Duration> Durations { get; set; }
        public DbSet<Models.Keyword> Keywords { get; set; }
    }
}