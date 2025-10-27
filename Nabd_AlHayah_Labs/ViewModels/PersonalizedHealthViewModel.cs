using Nabd_AlHayah_Labs.Model;

namespace Nabd_AlHayah_Labs.ViewModels
{
    public class PersonalizedHealthViewModel
    {
        public int Id { get; set; }
        public int TestId { get; set; }

        public IFormFile? CardImageFile { get; set; }
        public IFormFile? TestImageFile { get; set; }

        public byte[]? CardImage { get; set; }
        public byte[]? TestImage { get; set; }

        public string? CardTitleAr { get; set; }
        public string? CardTitleEn { get; set; }
        public string? CardSnippetAr { get; set; }
        public string? CardSnippetEn { get; set; }
        public string? TestNameAr { get; set; }
        public string? TestNameEn { get; set; }
        public string? DescriptionAr { get; set; }
        public string? DescriptionEn { get; set; }
        public string? RequirementsAr { get; set; }
        public string? RequirementsEn { get; set; }

        public List<Test> AllTests { get; set; } = new List<Test>();
    }
}
