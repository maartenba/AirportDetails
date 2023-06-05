using Microsoft.EntityFrameworkCore;

namespace AirportDetails;

public class Database : DbContext
{
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Airport> Airports => Set<Airport>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite(@"Data Source=../../../db.sqlite");

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Country indices
        builder.Entity<Country>().HasIndex(nameof(Country.IsoCode));
        builder.Entity<Country>().HasIndex(nameof(Country.NormalizedIsoCode));

        // Airport indices
        builder.Entity<Airport>().HasIndex(nameof(Airport.Iata));
        builder.Entity<Airport>().HasIndex(nameof(Airport.NormalizedIata));
        builder.Entity<Airport>().HasIndex(nameof(Airport.City));
        builder.Entity<Airport>().HasIndex(nameof(Airport.NormalizedCity));
    }
}