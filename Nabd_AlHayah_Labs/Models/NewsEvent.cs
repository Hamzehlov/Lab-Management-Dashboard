using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class NewsEvent
{
    public int Id { get; set; }

    public string TitleAr { get; set; } = null!;

    public string? TitleEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? EventDate { get; set; }
    public bool? IsActive { get; set; }

    public byte[]? Image { get; set; }
}
