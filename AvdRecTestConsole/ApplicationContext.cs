using GrpcService.Models;
using Microsoft.EntityFrameworkCore;
using Crud;  // пространство имен сервиса WorkerService.WorkerServiceBase


namespace AvdRecTestConsole
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Worker> Workers { get; set; } = null!;

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=yfrfpe.obq");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<Sex>();
            modelBuilder.Entity<Worker>().HasData(
                    new Worker { Id = 1, LastName = "Tom", Birthday = 37, Sex = Sex.Male }
            );
        }
    }
}
