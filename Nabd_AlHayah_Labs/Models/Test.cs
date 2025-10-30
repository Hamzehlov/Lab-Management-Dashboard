using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class Test
{
    public int TestId { get; set; }

    public string TestNameAr { get; set; } = null!;

    public string? TestNameEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    public string? RequirementsAr { get; set; }

    public string? RequirementsEn { get; set; }

    public string? ShortBenefitAr { get; set; }

    public string? ShortBenefitEn { get; set; }

    public int? CategoryId { get; set; }

    public byte[]? TestImage { get; set; }

    public decimal? Price { get; set; }
    public bool? IsActive { get; set; } 


    public virtual ICollection<AppointmentTest> AppointmentTests { get; set; } = new List<AppointmentTest>();

    public virtual Code? Category { get; set; }

    public virtual ICollection<PackageTest> PackageTests { get; set; } = new List<PackageTest>();

    public virtual ICollection<PersonalizedHealth> PersonalizedHealths { get; set; } = new List<PersonalizedHealth>();
}
