using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class PackageTest
{
    public int Id { get; set; }

    public int PackageId { get; set; }

    public int TestId { get; set; }

    public virtual HealthPackage Package { get; set; } = null!;

    public virtual Test Test { get; set; } = null!;
}
