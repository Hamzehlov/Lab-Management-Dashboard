using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class AppointmentPackage
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int PackageId { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual HealthPackage Package { get; set; } = null!;
}
