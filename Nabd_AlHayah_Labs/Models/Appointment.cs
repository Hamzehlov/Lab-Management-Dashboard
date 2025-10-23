using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? PatientId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public int? AppointmentTypeId { get; set; }

    public int? StatusId { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? FirstNameAr { get; set; }

    public string? FirstNameEn { get; set; }

    public string? MiddleNameAr { get; set; }

    public string? MiddleNameEn { get; set; }

    public string? LastNameAr { get; set; }

    public string? LastNameEn { get; set; }

    public int? GenderId { get; set; }

    public DateOnly? BirthDate { get; set; }

    public virtual ICollection<AppointmentPackage> AppointmentPackages { get; set; } = new List<AppointmentPackage>();

    public virtual ICollection<AppointmentTest> AppointmentTests { get; set; } = new List<AppointmentTest>();

    public virtual Code? AppointmentType { get; set; }

    public virtual Code? Gender { get; set; }

    public virtual ICollection<HomeSampling> HomeSamplings { get; set; } = new List<HomeSampling>();

    public virtual Patient? Patient { get; set; }

    public virtual Code? Status { get; set; }
}
