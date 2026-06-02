using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShippingService.Models
{
    [Table("codes")]
    public class Code
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("uuid")]
        [JsonPropertyName("uuid")]
        public long Uuid { get; set; }

        [Column("code")]
        [JsonPropertyName("code")]
        public string? CodeValue { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
