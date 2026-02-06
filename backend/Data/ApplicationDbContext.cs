using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Vehicle> Vehicles { get; set; } = null!;
    public DbSet<ServiceRecord> ServiceRecords { get; set; } = null!;
    public DbSet<FuelRecord> FuelRecords { get; set; } = null!;
    public DbSet<CarMake> CarMakes { get; set; } = null!;
    public DbSet<CarModel> CarModels { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Vehicle configuration
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasOne(v => v.User)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.LicensePlate);
            entity.HasIndex(e => e.VIN).IsUnique();
        });

        // ServiceRecord configuration
        modelBuilder.Entity<ServiceRecord>(entity =>
        {
            entity.HasOne(sr => sr.Vehicle)
                .WithMany(v => v.ServiceRecords)
                .HasForeignKey(sr => sr.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ServiceDate);
        });

        // FuelRecord configuration
        modelBuilder.Entity<FuelRecord>(entity =>
        {
            entity.HasOne(fr => fr.Vehicle)
                .WithMany(v => v.FuelRecords)
                .HasForeignKey(fr => fr.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.RefuelDate);
        });

        // CarMake and CarModel configuration
        modelBuilder.Entity<CarModel>(entity =>
        {
            entity.HasOne(cm => cm.Make)
                .WithMany(m => m.Models)
                .HasForeignKey(cm => cm.MakeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data for car makes and models
        SeedCarMakesAndModels(modelBuilder);
    }

    private void SeedCarMakesAndModels(ModelBuilder modelBuilder)
    {
        // Seed Car Makes
        var makes = new[]
        {
            new CarMake { Id = 1, Name = "Toyota" },
            new CarMake { Id = 2, Name = "Honda" },
            new CarMake { Id = 3, Name = "Ford" },
            new CarMake { Id = 4, Name = "Chevrolet" },
            new CarMake { Id = 5, Name = "BMW" },
            new CarMake { Id = 6, Name = "Mercedes-Benz" },
            new CarMake { Id = 7, Name = "Audi" },
            new CarMake { Id = 8, Name = "Volkswagen" },
            new CarMake { Id = 9, Name = "Nissan" },
            new CarMake { Id = 10, Name = "Hyundai" }
        };

        modelBuilder.Entity<CarMake>().HasData(makes);

        // Seed Car Models
        var models = new List<CarModel>
        {
            // Toyota models
            new CarModel { Id = 1, MakeId = 1, Name = "Camry" },
            new CarModel { Id = 2, MakeId = 1, Name = "Corolla" },
            new CarModel { Id = 3, MakeId = 1, Name = "RAV4" },
            new CarModel { Id = 4, MakeId = 1, Name = "Highlander" },
            new CarModel { Id = 5, MakeId = 1, Name = "Prius" },
            new CarModel { Id = 6, MakeId = 1, Name = "Tacoma" },
            new CarModel { Id = 7, MakeId = 1, Name = "4Runner" },

            // Honda models
            new CarModel { Id = 8, MakeId = 2, Name = "Civic" },
            new CarModel { Id = 9, MakeId = 2, Name = "Accord" },
            new CarModel { Id = 10, MakeId = 2, Name = "CR-V" },
            new CarModel { Id = 11, MakeId = 2, Name = "Pilot" },
            new CarModel { Id = 12, MakeId = 2, Name = "Odyssey" },
            new CarModel { Id = 13, MakeId = 2, Name = "HR-V" },

            // Ford models
            new CarModel { Id = 14, MakeId = 3, Name = "F-150" },
            new CarModel { Id = 15, MakeId = 3, Name = "Mustang" },
            new CarModel { Id = 16, MakeId = 3, Name = "Explorer" },
            new CarModel { Id = 17, MakeId = 3, Name = "Escape" },
            new CarModel { Id = 18, MakeId = 3, Name = "Edge" },
            new CarModel { Id = 19, MakeId = 3, Name = "Bronco" },

            // Chevrolet models
            new CarModel { Id = 20, MakeId = 4, Name = "Silverado" },
            new CarModel { Id = 21, MakeId = 4, Name = "Equinox" },
            new CarModel { Id = 22, MakeId = 4, Name = "Malibu" },
            new CarModel { Id = 23, MakeId = 4, Name = "Traverse" },
            new CarModel { Id = 24, MakeId = 4, Name = "Tahoe" },
            new CarModel { Id = 25, MakeId = 4, Name = "Camaro" },

            // BMW models
            new CarModel { Id = 26, MakeId = 5, Name = "3 Series" },
            new CarModel { Id = 27, MakeId = 5, Name = "5 Series" },
            new CarModel { Id = 28, MakeId = 5, Name = "X3" },
            new CarModel { Id = 29, MakeId = 5, Name = "X5" },
            new CarModel { Id = 30, MakeId = 5, Name = "X7" },
            new CarModel { Id = 31, MakeId = 5, Name = "M3" },

            // Mercedes-Benz models
            new CarModel { Id = 32, MakeId = 6, Name = "C-Class" },
            new CarModel { Id = 33, MakeId = 6, Name = "E-Class" },
            new CarModel { Id = 34, MakeId = 6, Name = "GLC" },
            new CarModel { Id = 35, MakeId = 6, Name = "GLE" },
            new CarModel { Id = 36, MakeId = 6, Name = "S-Class" },
            new CarModel { Id = 37, MakeId = 6, Name = "GLA" },

            // Audi models
            new CarModel { Id = 38, MakeId = 7, Name = "A4" },
            new CarModel { Id = 39, MakeId = 7, Name = "A6" },
            new CarModel { Id = 40, MakeId = 7, Name = "Q5" },
            new CarModel { Id = 41, MakeId = 7, Name = "Q7" },
            new CarModel { Id = 42, MakeId = 7, Name = "A3" },
            new CarModel { Id = 43, MakeId = 7, Name = "Q3" },

            // Volkswagen models
            new CarModel { Id = 44, MakeId = 8, Name = "Jetta" },
            new CarModel { Id = 45, MakeId = 8, Name = "Passat" },
            new CarModel { Id = 46, MakeId = 8, Name = "Tiguan" },
            new CarModel { Id = 47, MakeId = 8, Name = "Atlas" },
            new CarModel { Id = 48, MakeId = 8, Name = "Golf" },
            new CarModel { Id = 49, MakeId = 8, Name = "Taos" },

            // Nissan models
            new CarModel { Id = 50, MakeId = 9, Name = "Altima" },
            new CarModel { Id = 51, MakeId = 9, Name = "Sentra" },
            new CarModel { Id = 52, MakeId = 9, Name = "Rogue" },
            new CarModel { Id = 53, MakeId = 9, Name = "Pathfinder" },
            new CarModel { Id = 54, MakeId = 9, Name = "Murano" },
            new CarModel { Id = 55, MakeId = 9, Name = "Frontier" },

            // Hyundai models
            new CarModel { Id = 56, MakeId = 10, Name = "Elantra" },
            new CarModel { Id = 57, MakeId = 10, Name = "Sonata" },
            new CarModel { Id = 58, MakeId = 10, Name = "Tucson" },
            new CarModel { Id = 59, MakeId = 10, Name = "Santa Fe" },
            new CarModel { Id = 60, MakeId = 10, Name = "Palisade" },
            new CarModel { Id = 61, MakeId = 10, Name = "Kona" }
        };

        modelBuilder.Entity<CarModel>().HasData(models);
    }
}
