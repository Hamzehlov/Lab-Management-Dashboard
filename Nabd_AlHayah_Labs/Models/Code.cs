using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class Code
{
    public int Id { get; set; }

    public string CodeDescAr { get; set; } = null!;

    public string? CodeDescEn { get; set; }

    public int? ParentId { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<Appointment> AppointmentAppointmentTypes { get; set; } = new List<Appointment>();

    public virtual ICollection<Appointment> AppointmentGenders { get; set; } = new List<Appointment>();

    public virtual ICollection<Appointment> AppointmentStatuses { get; set; } = new List<Appointment>();

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
