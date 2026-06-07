using Microsoft.EntityFrameworkCore;
using SmartHomeEnergyDashboard.DAL.Entities;

namespace SmartHomeEnergyDashboard.DAL
{
    public class ProducersDbContext : DbContext
    {
        private readonly string _dbPath;
        public DbSet<ProducerEntity> Producers { get; set; }

        public ProducersDbContext(string dbPath) => _dbPath = dbPath;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }
}
