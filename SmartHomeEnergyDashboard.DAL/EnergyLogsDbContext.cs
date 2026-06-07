using Microsoft.EntityFrameworkCore;
using SmartHomeEnergyDashboard.DAL.Entities;

namespace SmartHomeEnergyDashboard.DAL
{
    public class EnergyLogsDbContext : DbContext
    {
        private readonly string _dbPath;
        public DbSet<EnergyLogEntity> EnergyLogs { get; set; }

        public EnergyLogsDbContext(string dbPath) => _dbPath = dbPath;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }
}
