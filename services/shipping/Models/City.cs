using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShippingService.Models
{
    [Table("cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("uuid")]
        [JsonPropertyName("uuid")]
        public long Uuid { get; set; }

        [Column("country_code")]
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [Column("city")]
        [JsonPropertyName("city")]
        public string? CityName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
