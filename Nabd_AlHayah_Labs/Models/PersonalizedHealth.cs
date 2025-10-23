using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class PersonalizedHealth
{
    public int Id { get; set; }

    public int TestId { get; set; }

    public byte[]? CardImage { get; set; }

    public string? CardTitleAr { get; set; }

    public string? CardTitleEn { get; set; }

    public string? CardSnippetAr { get; set; }

    public string? CardSnippetEn { get; set; }

    public byte[]? TestImage { get; set; }

    public string? TestNameAr { get; set; }

    public string? TestNameEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    public string? RequirementsAr { get; set; }

    public string? RequirementsEn { get; set; }

    public virtual Test Test { get; set; } = null!;
}
