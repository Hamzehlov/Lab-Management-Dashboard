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

		[Required(ErrorMessage = "Please enter a password.")]
		public string Password { get; set; } = null!;


		// قائمة الجندر لعمل DropDownList
		public List<SelectListItem> Genders { get; set; } = new List<SelectListItem>();
    }

    public class HealthMonitoringViewModel
    {
        public Patient Patient { get; set; } = new Patient();
        public List<HealthMonitoring> HealthMonitorings { get; set; } = new List<HealthMonitoring>();

        // جديد لإضافة/تعديل سجل
        public HealthMonitoring NewRecord { get; set; } = new HealthMonitoring();
    }

    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public Patient Patient { get; set; }
		public int PatientId { get; set; }

		public List<Appointment>? Appointments { get; set; } = new();
        public List<SelectableItem> Packages { get; set; } = new();
        public List<SelectableItem> Tests { get; set; } = new();

        // بيانات الموعد
        public int AppointmentTypeId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Notes { get; set; }

        public string? Appointmentdesc { get; set; }

        // بيانات السحب المنزلي
        public string? AddressAr { get; set; }
        public string? AddressEn { get; set; }
        public string? CityAr { get; set; }
        public string? CityEn { get; set; }
        public string? TechnicianName { get; set; }
        public bool IsForAnotherPerson { get; set; }

        public string? Statusdesc { get; set; }   // <--- هذه الخاصية مفقودة



        // الفحوصات والباقات المختارة
        public List<int> SelectedTestIds { get; set; } = new();
        public List<int> SelectedPackageIds { get; set; } = new();


        public List<SelectableItem> AppointmentTypes { get; set; } = new();

    }

    public class SelectableItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Selected { get; set; } = false;
    }


}
