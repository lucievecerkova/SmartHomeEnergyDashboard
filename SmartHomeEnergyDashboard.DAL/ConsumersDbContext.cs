using Microsoft.EntityFrameworkCore;
using SmartHomeEnergyDashboard.DAL.Entities;

namespace SmartHomeEnergyDashboard.DAL
{
    public class ConsumersDbContext : DbContext
    {
        private readonly string _dbPath;
        public DbSet<ConsumerEntity> Consumers { get; set; }

        public ConsumersDbContext(string dbPath) => _dbPath = dbPath;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={_dbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ConsumerEntity>().HasData(
                new ConsumerEntity { Id = 1, Name = "Fridge", Type = "Kitchen", Power = 150 },
                new ConsumerEntity { Id = 2, Name = "TV", Type = "Living room", Power = 80 },
                new ConsumerEntity { Id = 3, Name = "Washing mashine", Type = "Bathroom", Power = 500 }
            );
        }
    }
}
