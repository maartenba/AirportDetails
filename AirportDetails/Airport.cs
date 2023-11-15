using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportDetails;

public class Airport
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public required string NormalizedName { get; set; }

    public Country? Country { get; set; }

    [Required, MaxLength(200)]
    public required string City { get; set; }

    [Required, MaxLength(200)]
    public required string NormalizedCity { get; set; }

    [MaxLength(20)]
    public string? Iata { get; set; }

    [MaxLength(20)]
    public string? NormalizedIata { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,5)")]
    public decimal Latitude { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,5)")]
    public decimal Longitude { get; set; }

    [MaxLength(60)]
    public string? Timezone { get; set; }

    public List<Airline> Airlines { get; set; } = new();
}