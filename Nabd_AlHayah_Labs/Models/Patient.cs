using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class Patient
{
    public int PatientId { get; set; }

    public string? FullNameAr { get; set; }

    public string? FullNameEn { get; set; }

    public string? NationalId { get; set; }

    public DateOnly? BirthDate { get; set; }

    public int? GenderId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? AddressAr { get; set; }

    public string? AddressEn { get; set; }

    public string? GovernorateAr { get; set; }

    public string? GovernorateEn { get; set; }

    public string? CityAr { get; set; }

    public string? CityEn { get; set; }

    public string PasswordHash { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? EmergencyContact { get; set; }

    public string? BloodType { get; set; }

    public int? InsuranceCompanyId { get; set; }

    public string? InsuranceNumber { get; set; }

    public string? MoHhealthNumber { get; set; }

    public DateTime? LastVisitDate { get; set; }

    public string? ActivationCode { get; set; }

    public bool? EmailVerified { get; set; }

    public int? Pat_No { get; set; }


    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Code? Gender { get; set; }

    public virtual ICollection<HealthMonitoring> HealthMonitorings { get; set; } = new List<HealthMonitoring>();
}
