using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BOs;

public partial class SilverJewelry
{
    public string SilverJewelryId { get; set; } = null!;

    public string SilverJewelryName { get; set; } = null!;

    public string SilverJewelryDescription { get; set; } = null!;

    public decimal MetalWeight { get; set; }  = default!;

    public decimal Price { get; set; } = default!;

    public int ProductionYear { get; set; } = 0!;

    public DateTime? CreatedDate { get; set; }

    public string? CategoryId { get; set; }

    [JsonIgnore]
    public virtual Category? Category { get; set; }
}
