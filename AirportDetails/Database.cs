using Microsoft.EntityFrameworkCore;

namespace AirportDetails;

public class Database : DbContext
{
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Airline> Airlines => Set<Airline>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(@"Data Source=../../../db.sqlite");

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Data options
        builder.Entity<Country>().Property(ci => ci.Name)
            .HasMaxLength(100);

        // Country indices
        builder.Entity<Country>().HasIndex(nameof(Country.IsoCode));
        builder.Entity<Country>().HasIndex(nameof(Country.NormalizedIsoCode));

        // Airport indices
        builder.Entity<Airport>().HasIndex(nameof(Airport.Iata));
        builder.Entity<Airport>().HasIndex(nameof(Airport.NormalizedIata));
        builder.Entity<Airport>().HasIndex(nameof(Airport.City));
        builder.Entity<Airport>().HasIndex(nameof(Airport.NormalizedCity));

        // Airline indices
        builder.Entity<Airline>().HasIndex(nameof(Airline.Iata));
        builder.Entity<Airline>().HasIndex(nameof(Airline.NormalizedIata));
    }
}