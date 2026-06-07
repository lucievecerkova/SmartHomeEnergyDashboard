using Microsoft.EntityFrameworkCore;
using SmartHomeEnergyDashboard.DAL;
using SmartHomeEnergyDashboard.DAL.Entities;
using SmartHomeEnergyDashboard.DTOs;

namespace SmartHomeEnergyDashboard.Services
{
    public class EnergyDashboardService
    {
        private readonly string _producersPath;
        private readonly string _consumersPath;
        private readonly string _logsPath;

        public EnergyDashboardService()
        {
            _producersPath = Path.Combine(FileSystem.AppDataDirectory, "Producers564364.db3");
            _consumersPath = Path.Combine(FileSystem.AppDataDirectory, "Consumers564364.db3");
            _logsPath = Path.Combine(FileSystem.AppDataDirectory, "EnergyLogs564364.db3");

            /*using (var db = new ProducersDbContext(_producersPath)) db.Database.EnsureDeleted();
            using (var db = new ConsumersDbContext(_consumersPath)) db.Database.EnsureDeleted();
            using (var db = new EnergyLogsDbContext(_logsPath)) db.Database.EnsureDeleted();*/

            using (var db = new ProducersDbContext(_producersPath)) db.Database.EnsureCreated();
            using (var db = new ConsumersDbContext(_consumersPath)) db.Database.EnsureCreated();
            using (var db = new EnergyLogsDbContext(_logsPath)) db.Database.EnsureCreated();
        }

        public async Task<List<ProducerDTO>> GetProducersAsync()
        {
            using var context = new ProducersDbContext(_producersPath);
            var entities = await context.Producers.ToListAsync();

            return entities.Select(e => new ProducerDTO
            {
                Id = e.Id,
                Power = e.Power
            }).ToList();
        }

        public async Task<int> DeleteProducerAsync(ProducerDTO deviceDto)
        {
            if (deviceDto == null) return 0;
            using var context = new ProducersDbContext(_producersPath);
            var entity = new ProducerEntity { Id = deviceDto.Id };
            context.Producers.Remove(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<int> SaveProducerRecordAsync(ProducerDTO deviceDto)
        {
            using var context = new ProducersDbContext(_producersPath);
            var entity = new ProducerEntity
            {
                Id = deviceDto.Id,
                Power = deviceDto.Power
            };
            if (entity.Id == 0)
            {
                context.Entry(entity).State = EntityState.Added;
            }
            else
            {
                context.Entry(entity).State = EntityState.Modified;
            }

            int result = await context.SaveChangesAsync();
            if (deviceDto.Id == 0)
            {
                deviceDto.Id = entity.Id;
            }

            return result; 
        }

        public async Task<List<ConsumerDTO>> GetConsumersAsync()
        {
            using var context = new ConsumersDbContext(_consumersPath);
            var entities = await context.Consumers.ToListAsync();

            return entities.Select(e => new ConsumerDTO
            {
                Id = e.Id,
                Power = e.Power,
                Name = e.Name,
                Type = e.Type
            }).ToList();
        }

        public async Task<int> DeleteConsumerAsync(ConsumerDTO deviceDto)
        {
            if (deviceDto == null) return 0;
            using var context = new ConsumersDbContext(_consumersPath);
            var entity = new ConsumerEntity { Id = deviceDto.Id };
            context.Consumers.Remove(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<int> SaveConsumerRecordAsync(ConsumerDTO deviceDto)
        {
            using var context = new ConsumersDbContext(_consumersPath);
            var entity = new ConsumerEntity
            {
                Id = deviceDto.Id,
                Power = deviceDto.Power,
                Name = deviceDto.Name,
                Type= deviceDto.Type
            };
            if (entity.Id == 0)
            {
                context.Entry(entity).State = EntityState.Added;
            }
            else
            {
                context.Entry(entity).State = EntityState.Modified;
            }

            int result = await context.SaveChangesAsync();
            if (deviceDto.Id == 0)
            {
                deviceDto.Id = entity.Id;
            }

            return result;
        }

        public async Task<List<EnergyLogDTO>> GetLogsAsync()
        {
            using var context = new EnergyLogsDbContext(_logsPath);

            // get last 50
            var entities = await context.EnergyLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(72)
                .ToListAsync();
            entities.Reverse();

            return entities.Select(e => new EnergyLogDTO
            {
                Id = e.Id,
                Timestamp = e.Timestamp,
                SimulatedHour = e.SimulatedHour,
                Production = e.Production,
                Consumption = e.Consumption,
                Balance = e.Balance,
                BatteryLevel = e.BatteryLevel
            }).ToList();
        }

        public async Task AddLogAsync(EnergyLogDTO dto)
        {
            using var context = new EnergyLogsDbContext(_logsPath);

            var entity = new EnergyLogEntity
            {
                Timestamp = dto.Timestamp,
                SimulatedHour = dto.SimulatedHour,
                Production = dto.Production,
                Consumption = dto.Consumption,
                Balance = dto.Balance,
                BatteryLevel = dto.BatteryLevel
            };

            context.EnergyLogs.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task<int> ClearAllLogsAsync()
        {
            using var context = new EnergyLogsDbContext(_logsPath);
            int rowsDeleted = await context.EnergyLogs.ExecuteDeleteAsync();
            return rowsDeleted;
        }
    }
}
