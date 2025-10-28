namespace Nabd_AlHayah_Labs.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int ApprovedPatients { get; set; }
        public int PendingPatients { get; set; }

        public double ApprovedPercentage =>
            TotalPatients > 0 ? Math.Round((ApprovedPatients * 100.0) / TotalPatients, 1) : 0;
    }
}
