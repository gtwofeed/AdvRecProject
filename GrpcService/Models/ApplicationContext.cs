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
                new Worker { Id = 1, LastName = "Tom", FirstName = "Soyer", MiddleName = "Tomi", Birthday = 631255680000000000, Sex = Sex.Male, HasChildren = true },
                new Worker { Id = 2, LastName = "Kety", FirstName = "Soyer", MiddleName = "Trunk", Birthday = 631355680000000000, Sex = Sex.Female, HasChildren = true },
                new Worker { Id = 3, LastName = "TN", FirstName = "Soydfer", MiddleName = "Tofdmi", Birthday = 63125568000000, HasChildren = false  }
        );
    }
}
