using Microsoft.AspNetCore.Mvc.Rendering;
using Nabd_AlHayah_Labs.Model;

namespace Nabd_AlHayah_Labs.ViewModels
{
    public class TestViewModel
    {
        public int TestId { get; set; }
        public string TestNameAr { get; set; } = string.Empty;
        public string? TestNameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? RequirementsAr { get; set; }
        public string? RequirementsEn { get; set; }
        public string? ShortBenefitAr { get; set; }
        public string? ShortBenefitEn { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Price { get; set; }

        // صورة الفحص
        public IFormFile? TestImageFile { get; set; }
        public byte[]? TestImage { get; set; }

   
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

    }
}
