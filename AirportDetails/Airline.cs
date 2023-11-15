using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportDetails;

public class Airline
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public required string NormalizedName { get; set; }

    [MaxLength(20)]
    public string? Iata { get; set; }

    [MaxLength(20)]
    public string? NormalizedIata { get; set; }

    public string? Callsign { get; set; }

    public Country? Country { get; set; }

    public List<Airport> Airports { get; set; } = new();
}