using Nabd_AlHayah_Labs.Model;

namespace Nabd_AlHayah_Labs.ViewModels
{
    public class PackageViewModel
    {
        public int PackageId { get; set; }
        public string PackageNameAr { get; set; } = string.Empty;
        public string? PackageNameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public decimal Price { get; set; }
        public string? Duration { get; set; }
        public string? RequirementsEn { get; set; } // <-- أضف هذه الخاصية
        public string? RequirementsAr { get; set; } // <-- أضف هذه الخاصية
        public bool IsActive { get; set; }

        public List<int> SelectedTestIds { get; set; } = new List<int>();
        public List<Test> AllTests { get; set; } = new List<Test>();

        // صورة الباقة
        public IFormFile? PackageImageFile { get; set; }
        public byte[]? PackageImage { get; set; }
    }
}
