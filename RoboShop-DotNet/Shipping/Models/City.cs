using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboShop.Shipping.Models;

/// <summary>
/// Entity for City — converted from Java JPA @Entity City.java
/// </summary>
[Table("cities")]
public class City
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Uuid { get; set; }

    /// <summary>Maps to country_code column (was @Column(name = "country_code") in Java)</summary>
    [Column("country_code")]
    public string Code { get; set; } = string.Empty;

    public string CityName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public override string ToString() =>
        $"Country: {Code} City: {CityName} Region: {Region} Coords: {Latitude} {Longitude}";
}
