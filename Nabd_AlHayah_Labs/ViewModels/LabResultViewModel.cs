namespace Nabd_AlHayah_Labs.ViewModels
{
    public class LabResultViewModel
    {
        public int Pat_No { get; set; }
        public string Pat_Id { get; set; } = string.Empty;
        public string Pat_FName { get; set; } = string.Empty;
        public DateTime Pat_DateOfBirth { get; set; }
        public string Pat_Gender { get; set; } = string.Empty;
        public string Pat_MobileNumber1 { get; set; } = string.Empty;
        public string ResultLink { get; set; } = string.Empty;

        // حقل مساعد لتنسيق التاريخ إذا أحببت
        public string DateOfBirthFormatted => Pat_DateOfBirth.ToString("yyyy-MM-dd");
    }
}
