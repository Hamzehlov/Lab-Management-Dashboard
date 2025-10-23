using System;
using System.Collections.Generic;

namespace Nabd_AlHayah_Labs.Model;

public partial class HealthMonitoring
{
    public int MonitorId { get; set; }

    public int? PatientId { get; set; }

    public string RecommendationTitleAr { get; set; } = null!;

    public string? RecommendationTitleEn { get; set; }

    public string RecommendationDetailsAr { get; set; } = null!;

    public string? RecommendationDetailsEn { get; set; }

    public string? BasedOnTests { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Patient? Patient { get; set; }
}
