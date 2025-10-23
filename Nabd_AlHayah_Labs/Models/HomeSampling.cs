using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class HomeSampling
{
    public int HomeSampleId { get; set; }

    public int? AppointmentId { get; set; }

    public string AddressAr { get; set; } = null!;

    public string? AddressEn { get; set; }

    public string? CityAr { get; set; }

    public string? CityEn { get; set; }

    public string? TechnicianName { get; set; }

    public DateTime VisitTime { get; set; }

    public bool? IsForAnotherPerson { get; set; }

    public virtual Appointment? Appointment { get; set; }
}
