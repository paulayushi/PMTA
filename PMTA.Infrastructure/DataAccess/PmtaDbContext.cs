using Microsoft.EntityFrameworkCore;
using PMTA.Domain.Entity;

namespace PMTA.Infrastructure.DataAccess
{
    public class PmtaDbContext: DbContext
    {
        public PmtaDbContext(DbContextOptions<PmtaDbContext> options): base(options)
        {
        }

        public DbSet<MemberEntity> Members { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        //public DbSet<UserEntity> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
