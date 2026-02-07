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
    public DbSet<MaintenanceTemplate> MaintenanceTemplates { get; set; } = null!;
    public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; } = null!;
    public DbSet<Reminder> Reminders { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

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

        // MaintenanceTemplate configuration
        modelBuilder.Entity<MaintenanceTemplate>(entity =>
        {
            entity.HasOne(mt => mt.User)
                .WithMany()
                .HasForeignKey(mt => mt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.Category);
        });

        // MaintenanceSchedule configuration
        modelBuilder.Entity<MaintenanceSchedule>(entity =>
        {
            entity.HasOne(ms => ms.Vehicle)
                .WithMany(v => v.MaintenanceSchedules)
                .HasForeignKey(ms => ms.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ms => ms.Template)
                .WithMany(t => t.MaintenanceSchedules)
                .HasForeignKey(ms => ms.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(ms => ms.LastServiceRecord)
                .WithMany()
                .HasForeignKey(ms => ms.LastServiceRecordId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.NextDueDate);
        });

        // Reminder configuration
        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasOne(r => r.MaintenanceSchedule)
                .WithMany(ms => ms.Reminders)
                .HasForeignKey(r => r.MaintenanceScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ScheduledDate);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(n => n.Reminder)
                .WithMany(r => r.Notifications)
                .HasForeignKey(n => n.ReminderId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Channel);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Seed data for car makes and models
        SeedCarMakesAndModels(modelBuilder);

        // Seed data for system maintenance templates
        SeedMaintenanceTemplates(modelBuilder);
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

    private void SeedMaintenanceTemplates(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2026, 2, 7, 0, 0, 0, DateTimeKind.Utc);
        var templates = new List<MaintenanceTemplate>
        {
            // Engine Maintenance
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Oil Change",
                Description = "Regular oil and filter change",
                Category = "Engine",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 6,
                DefaultIntervalKilometers = 10000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Oil Filter Replacement",
                Description = "Replace engine oil filter",
                Category = "Engine",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 6,
                DefaultIntervalKilometers = 10000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Air Filter Replacement",
                Description = "Replace engine air filter",
                Category = "Engine",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 12,
                DefaultIntervalKilometers = 20000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "Spark Plugs Replacement",
                Description = "Replace spark plugs",
                Category = "Engine",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 24,
                DefaultIntervalKilometers = 50000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                Name = "Timing Belt Replacement",
                Description = "Replace timing belt",
                Category = "Engine",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 60,
                DefaultIntervalKilometers = 100000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },

            // Tires & Wheels
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000006"),
                Name = "Tire Rotation",
                Description = "Rotate tires for even wear",
                Category = "Tires",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 6,
                DefaultIntervalKilometers = 10000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000007"),
                Name = "Tire Replacement",
                Description = "Replace worn tires",
                Category = "Tires",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 48,
                DefaultIntervalKilometers = 60000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000008"),
                Name = "Wheel Alignment",
                Description = "Check and adjust wheel alignment",
                Category = "Tires",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 12,
                DefaultIntervalKilometers = 20000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000009"),
                Name = "Tire Pressure Check",
                Description = "Check and adjust tire pressure",
                Category = "Tires",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 1,
                DefaultIntervalKilometers = 2000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },

            // Brakes
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000010"),
                Name = "Brake Pad Inspection",
                Description = "Inspect brake pads and rotors",
                Category = "Brakes",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 12,
                DefaultIntervalKilometers = 20000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                Name = "Brake Fluid Replacement",
                Description = "Flush and replace brake fluid",
                Category = "Brakes",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 24,
                DefaultIntervalKilometers = 40000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },

            // Fluids
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                Name = "Coolant Flush",
                Description = "Flush and replace engine coolant",
                Category = "Fluids",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 24,
                DefaultIntervalKilometers = 40000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000013"),
                Name = "Transmission Fluid Change",
                Description = "Change transmission fluid",
                Category = "Fluids",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 48,
                DefaultIntervalKilometers = 80000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000014"),
                Name = "Power Steering Fluid",
                Description = "Check and replace power steering fluid",
                Category = "Fluids",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 24,
                DefaultIntervalKilometers = 40000,
                UseCompoundRule = true,
                CreatedAt = seedDate
            },

            // Inspections
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000015"),
                Name = "Annual Safety Inspection",
                Description = "Comprehensive vehicle safety inspection",
                Category = "Inspection",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 12,
                UseCompoundRule = false,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000016"),
                Name = "Emission Test",
                Description = "Emission test and certification",
                Category = "Inspection",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 24,
                UseCompoundRule = false,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000017"),
                Name = "Battery Check",
                Description = "Test battery and charging system",
                Category = "Inspection",
                IsSystemTemplate = true,
                DefaultIntervalMonths = 12,
                UseCompoundRule = false,
                CreatedAt = seedDate
            },

            // Equipment/Commercial (Engine Hours)
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000018"),
                Name = "Oil Change - Heavy Equipment",
                Description = "Oil change for heavy equipment based on engine hours",
                Category = "Equipment",
                IsSystemTemplate = true,
                DefaultIntervalHours = 250,
                UseCompoundRule = false,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000019"),
                Name = "Hydraulic Fluid Change",
                Description = "Change hydraulic fluid for equipment",
                Category = "Equipment",
                IsSystemTemplate = true,
                DefaultIntervalHours = 500,
                UseCompoundRule = false,
                CreatedAt = seedDate
            },
            new MaintenanceTemplate
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000020"),
                Name = "Air Filter - Equipment",
                Description = "Replace air filter on heavy equipment",
                Category = "Equipment",
                IsSystemTemplate = true,
                DefaultIntervalHours = 100,
                UseCompoundRule = false,
                CreatedAt = seedDate
            }
        };

        modelBuilder.Entity<MaintenanceTemplate>().HasData(templates);
    }
}
