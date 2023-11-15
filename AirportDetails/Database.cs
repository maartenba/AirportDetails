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
        // Country
        builder.Entity<Country>().Property(ci => ci.Name)
            .HasMaxLength(100);

        builder.Entity<Country>().HasIndex(nameof(Country.IsoCode));
        builder.Entity<Country>().HasIndex(nameof(Country.NormalizedIsoCode));

        // Airport
        builder.Entity<Airport>().HasMany(ci => ci.Airlines);

        builder.Entity<Airport>().HasIndex(nameof(Airport.Iata));
        builder.Entity<Airport>().HasIndex(nameof(Airport.NormalizedIata));
        builder.Entity<Airport>().HasIndex(nameof(Airport.City));
        builder.Entity<Airport>().HasIndex(nameof(Airport.NormalizedCity));

        // Airline
        builder.Entity<Airline>().HasMany(ci => ci.Airports);

        builder.Entity<Airline>().HasIndex(nameof(Airline.Iata));
        builder.Entity<Airline>().HasIndex(nameof(Airline.NormalizedIata));
    }
}