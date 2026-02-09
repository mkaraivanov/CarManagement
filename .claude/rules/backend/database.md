---
paths:
  - "backend/Data/**/*.cs"
  - "backend/Models/**/*.cs"
  - "backend/Migrations/**/*.cs"
---

# Database & Migration Best Practices

## Migration Workflow

### Creating Migrations

```bash
# After making model changes
cd backend
dotnet ef migrations add DescriptiveMigrationName
dotnet ef database update
```

### Critical Rules

- **Never modify existing migrations** that have been committed - create new ones instead
- Test migrations locally before committing
- Backup database before major schema changes: `cp backend/carmanagement.db backend/carmanagement.db.backup`

### If Migration Fails

```bash
# Rollback and fix
dotnet ef database update PreviousMigrationName
dotnet ef migrations remove

# Fix the issue, then create new migration
dotnet ef migrations add FixedMigrationName
dotnet ef database update
```

## Database Configuration

- **SQLite database file**: `backend/carmanagement.db`
- **Connection string**: `backend/appsettings.json`
- To reset database completely: delete file and run `dotnet ef database update`

### View Database

```bash
cd backend && sqlite3 carmanagement.db
```

### Reset Database

```bash
# Deletes all data
rm backend/carmanagement.db
cd backend && dotnet ef database update
```

## Entity Framework Core Patterns

### DbSet Registration

Add to `ApplicationDbContext.cs`:
```csharp
public DbSet<EntityName> EntityNames { get; set; }
```

### Relationship Configuration

Configure in `OnModelCreating()`:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<ParentEntity>()
        .HasMany(p => p.Children)
        .WithOne(c => c.Parent)
        .HasForeignKey(c => c.ParentId)
        .OnDelete(DeleteBehavior.Cascade);
}
```

## Database Seeding

- Car makes and models are seeded automatically in `ApplicationDbContext.cs`
- Seed data includes 10 makes and 60+ models
- To modify seed data, edit `SeedCarMakesAndModels()` method and create migration

## Common Mistakes

- ❌ Modifying existing migrations that have been committed
- ❌ Skipping migration creation after model changes
- ❌ Not configuring relationships in `OnModelCreating()`
- ❌ Forgetting to add DbSet for new entities
