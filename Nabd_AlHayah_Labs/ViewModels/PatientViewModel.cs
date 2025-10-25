using Microsoft.AspNetCore.Mvc.Rendering;
using Nabd_AlHayah_Labs.Model;
using System.ComponentModel.DataAnnotations;

namespace Nabd_AlHayah_Labs.ViewModels
{
	public class PatientViewModel
	{
		public int PatientId { get; set; }

		[Required(ErrorMessage = "الاسم الكامل بالعربية مطلوب")]
		[Display(Name = "الاسم الكامل (عربي)")]
		public string? FullNameAr { get; set; }

		[Display(Name = "الاسم الكامل (إنجليزي)")]
		public string? FullNameEn { get; set; }

		[Display(Name = "الرقم الوطني")]
		public string? NationalId { get; set; }

		[Display(Name = "تاريخ الميلاد")]
		[DataType(DataType.Date)]
		public DateTime? BirthDate { get; set; }

		[Display(Name = "الجنس")]
		public int? GenderId { get; set; }

		[Phone, Display(Name = "رقم الهاتف")]
		public string? Phone { get; set; }

		[EmailAddress, Display(Name = "البريد الإلكتروني")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "كلمة المرور مطلوبة")]
		[DataType(DataType.Password)]
		[Display(Name = "كلمة المرور")]
		public string Password { get; set; } = null!;

		[Display(Name = "العنوان (عربي)")]
		public string? AddressAr { get; set; }

		[Display(Name = "العنوان (إنجليزي)")]
		public string? AddressEn { get; set; }

		[Display(Name = "المحافظة (عربي)")]
		public string? GovernorateAr { get; set; }

		[Display(Name = "المحافظة (إنجليزي)")]
		public string? GovernorateEn { get; set; }

		[Display(Name = "المدينة (عربي)")]
		public string? CityAr { get; set; }

		[Display(Name = "المدينة (إنجليزي)")]
		public string? CityEn { get; set; }

		[Display(Name = "رقم الطوارئ")]
		public string? EmergencyContact { get; set; }

		[Display(Name = "فصيلة الدم")]
		public string? BloodType { get; set; }

		[Display(Name = "رقم التأمين")]
		public string? InsuranceNumber { get; set; }

		[Display(Name = "شركة التأمين")]
		public int? InsuranceCompanyId { get; set; }

		[Display(Name = "رقم وزارة الصحة")]
		public string? MoHhealthNumber { get; set; }

		[Display(Name = "آخر زيارة")]
		[DataType(DataType.Date)]
		public DateTime? LastVisitDate { get; set; }

		[Display(Name = "رمز التفعيل")]
		public string? ActivationCode { get; set; }

		[Display(Name = "تم التحقق من البريد؟")]
		public bool? EmailVerified { get; set; }

		[Display(Name = "رقم المريض")]
		public int? Pat_No { get; set; }

		[Display(Name = "نشط؟")]
		public bool IsActive { get; set; } = true;
	}

    public class PatientDetailsViewModel
    {
        public Patient Patient { get; set; } = new Patient();

        public IEnumerable<HealthMonitoring> HealthMonitorings { get; set; } = new List<HealthMonitoring>();

        public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();
    }


    public class PatientCreateViewModel
    {
        public Patient Patient { get; set; } = new Patient();

        // كلمة المرور بشكل منفصل
        public string? Password { get; set; }

        // قائمة الجندر لعمل DropDownList
        public List<SelectListItem> Genders { get; set; } = new List<SelectListItem>();
    }


}
