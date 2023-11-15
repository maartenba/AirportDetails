using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportDetails;

public class Country
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string NormalizedName { get; set; }

    [MaxLength(20)]
    public string? IsoCode { get; set; }

    [MaxLength(20)]
    public string? NormalizedIsoCode { get; set; }

    [InverseProperty(nameof(Airport.Country))]
    public List<Airport> Airports { get; set; } = new();

    [InverseProperty(nameof(Airline.Country))]
    public List<Airline> Airlines { get; set; } = new();
}