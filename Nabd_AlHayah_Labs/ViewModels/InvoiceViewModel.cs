namespace Nabd_AlHayah_Labs.ViewModels
{
    public class InvoiceViewModel
    {
        public int Pat_No { get; set; }
        public string VisM_Seq { get; set; } = string.Empty;
        public string VisM_No { get; set; } = string.Empty;
        public string Doc_Name { get; set; } = string.Empty;
        public DateTime VisM_Date { get; set; }
        public decimal VisM_Total { get; set; }
        public decimal VisM_Discount { get; set; }
        public decimal VisM_AmountToPay { get; set; }
        public decimal VisM_Remain { get; set; }

        // حقل مساعد لتنسيق التاريخ للعرض
        public string VisM_DateFormatted => VisM_Date.ToString("yyyy-MM-dd");
    }
}
