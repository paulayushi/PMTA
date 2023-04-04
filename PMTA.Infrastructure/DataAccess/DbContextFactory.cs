using Microsoft.EntityFrameworkCore;

namespace PMTA.Infrastructure.DataAccess
{
    public class DbContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;
        public DbContextFactory(Action<DbContextOptionsBuilder> configureDbContext) 
        { 
            _configureDbContext = configureDbContext;
        }

        public PmtaDbContext CreateDbContext()
        {
            DbContextOptionsBuilder<PmtaDbContext> optionsBuilder = new();
            _configureDbContext(optionsBuilder);
            return new PmtaDbContext(optionsBuilder.Options);
        }
    }
}
