using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoboShop.Shipping.Models;

/// <summary>
/// Entity for Code — converted from Java JPA @Entity Code.java
/// </summary>
[Table("codes")]
public class Code
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Uuid { get; set; }

    public string CodeValue { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public override string ToString() =>
        $"Code: {CodeValue} Name: {Name}";
}
