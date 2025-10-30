using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class HealthPackage
{
    public int PackageId { get; set; }

    public string PackageNameAr { get; set; } = null!;

    public string? PackageNameEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? DescriptionEn { get; set; }

    public decimal Price { get; set; }

    public string? Duration { get; set; }

    public string? RequirementsAr { get; set; }

    public string? RequirementsEn { get; set; }

    public string? ImageUrl { get; set; }

    public byte[]? PackageImage { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<AppointmentPackage> AppointmentPackages { get; set; } = new List<AppointmentPackage>();

    public virtual ICollection<PackageTest> PackageTests { get; set; } = new List<PackageTest>();
}
