using GrpcService.Models;
using Microsoft.EntityFrameworkCore;
using Crud;

public class ApplicationContext : DbContext
{
    public DbSet<Worker> Workers { get; set; } = null!;
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();   // создаем базу данных при первом обращении
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<Sex>();
        modelBuilder.Entity<Worker>().HasData(
                new Worker { Id = 1, LastName = "Tom", Birthday = 37, Sex = Sex.Male },
                new Worker { Id = 2, LastName = "Sara", Birthday = 40, Sex = Sex.Female },
                new Worker { Id = 4, LastName = "Styil", Birthday = 1000 }
        );
    }
}
