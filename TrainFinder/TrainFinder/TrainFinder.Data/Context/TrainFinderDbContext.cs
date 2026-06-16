using Microsoft.EntityFrameworkCore;
using TrainFinder.Data.Entities;

namespace TrainFinder.Data.Context;

public class TrainFinderDbContext : DbContext
{
    public TrainFinderDbContext(DbContextOptions<TrainFinderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Train> Trains => Set<Train>();

    public DbSet<Station> Stations => Set<Station>();

    public DbSet<TrainCurrentLocation> TrainCurrentLocations => Set<TrainCurrentLocation>();

    public DbSet<TrainLocationHistory> TrainLocationHistory => Set<TrainLocationHistory>();

    public DbSet<Timetable> Timetables => Set<Timetable>();

    public DbSet<TimetableStop> TimetableStops => Set<TimetableStop>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Request> Requests => Set<Request>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Train>()
            .HasIndex(x => x.TrainNumber)
            .IsUnique();

        modelBuilder.Entity<Station>()
            .HasIndex(x => x.Code)
            .IsUnique()
            .HasFilter("[Code] <> ''");

        modelBuilder.Entity<TrainCurrentLocation>()
            .HasIndex(x => x.TrainId)
            .IsUnique();

        modelBuilder.Entity<TrainCurrentLocation>()
            .HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TrainCurrentLocation>()
            .HasOne(x => x.NextStation)
            .WithMany()
            .HasForeignKey(x => x.NextStationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TrainLocationHistory>()
            .HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TrainLocationHistory>()
            .HasOne(x => x.NextStation)
            .WithMany()
            .HasForeignKey(x => x.NextStationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Timetable>()
            .HasIndex(x => x.TrainId)
            .IsUnique();

        modelBuilder.Entity<Timetable>()
            .HasOne(x => x.Train)
            .WithMany()
            .HasForeignKey(x => x.TrainId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimetableStop>()
            .HasOne(x => x.Timetable)
            .WithMany(x => x.Stops)
            .HasForeignKey(x => x.TimetableId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimetableStop>()
            .HasOne(x => x.Station)
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(x => x.ExternalObjectId).IsUnique();
            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.ExternalObjectId).IsRequired().HasMaxLength(100);
            entity.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(320);
            entity.Property(x => x.MobileNumber).HasMaxLength(20);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), Name = "User" },
            new Role { Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"), Name = "Admin" }
        );

        modelBuilder.Entity<Request>(entity =>
        {
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.Status).IsRequired();

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.UpdatedBy)
                .WithMany()
                .HasForeignKey(x => x.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}