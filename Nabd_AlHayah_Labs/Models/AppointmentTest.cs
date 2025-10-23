using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class AppointmentTest
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int TestId { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
