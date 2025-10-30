using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMedicalLaboratoryAPI.Data;
using Nabd_AlHayah_Labs.ViewModels;

namespace Nabd_AlHayah_Labs.Controllers
{
    public class AppointmentsController : Controller
    {

        private readonly MedicalLaboratoryDbContext _context;

        public AppointmentsController(MedicalLaboratoryDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            // جلب جميع المواعيد مع البيانات المرتبطة
            var appointments = await _context.Appointments
      .Include(a => a.AppointmentType)
      .Include(a => a.Status)
       .Include(a => a.Patient)// ← أضف هذا السطر
      .Include(a => a.AppointmentTests).ThenInclude(t => t.Test)
      .Include(a => a.AppointmentPackages).ThenInclude(p => p.Package)
      .Include(a => a.HomeSamplings)
      .OrderByDescending(a => a.AppointmentDate)
      .ToListAsync();

            // تعبئة ViewModel
            var viewModel = new AppointmentViewModel
            {
                Appointments = appointments,
                // إذا أردت يمكنك إضافة قائمة للـTypes وPackages وTests هنا أيضًا
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Pending_Appointments()
        {
            // جلب جميع المواعيد مع البيانات المرتبطة
      var appointments = await _context.Appointments.Where(x=>x.StatusId==7)
      .Include(a => a.AppointmentType)
      .Include(a => a.Status)
       .Include(a => a.Patient)// ← أضف هذا السطر
      .Include(a => a.AppointmentTests).ThenInclude(t => t.Test)
      .Include(a => a.AppointmentPackages).ThenInclude(p => p.Package)
      .Include(a => a.HomeSamplings)
      .OrderByDescending(a => a.AppointmentDate)
      .ToListAsync();

            // تعبئة ViewModel
            var viewModel = new AppointmentViewModel
            {
                Appointments = appointments,
                // إذا أردت يمكنك إضافة قائمة للـTypes وPackages وTests هنا أيضًا
            };

            return View(viewModel);
        }

        public async Task<IActionResult> AppointmentDetails(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.AppointmentType)
                .Include(a=>a.Status)// نوع الموعد
                .Include(a => a.AppointmentTests).ThenInclude(at => at.Test) // الفحوصات
                .Include(a => a.AppointmentPackages).ThenInclude(ap => ap.Package) // الباقات
                .Include(a => a.HomeSamplings) // السحب المنزلي
                .Include(a => a.Patient) // بيانات المريض
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
            {
                TempData["Error"] = "الموعد غير موجود.";
                return RedirectToAction("Appointment", new { patientId = 0 }); // عدل إذا تريد إعادة المريض
            }

            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                Appointmentdesc = appointment.AppointmentType?.CodeDescEn,
                Statusdesc = appointment.Status?.CodeDescEn,
                Patient = appointment.Patient,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTypeId = (int)appointment.AppointmentTypeId,
                Notes = appointment.Notes,
                SelectedTestIds = appointment.AppointmentTests.Select(t => t.TestId).ToList(),
                SelectedPackageIds = appointment.AppointmentPackages.Select(p => p.PackageId).ToList(),
                AddressAr = appointment.HomeSamplings.FirstOrDefault()?.AddressAr,
                AddressEn = appointment.HomeSamplings.FirstOrDefault()?.AddressEn,
                CityAr = appointment.HomeSamplings.FirstOrDefault()?.CityAr,
                CityEn = appointment.HomeSamplings.FirstOrDefault()?.CityEn,
                TechnicianName = appointment.HomeSamplings.FirstOrDefault()?.TechnicianName,
                IsForAnotherPerson = appointment.HomeSamplings.FirstOrDefault()?.IsForAnotherPerson ?? false,
                AppointmentTypes = await _context.Codes
                    .Where(c => c.ParentId == 10)
                    .Select(c => new SelectableItem
                    {
                        Id = c.Id,
                        Name = c.CodeDescEn + " / " + c.CodeDescAr
                    }).ToListAsync(),
                Packages = appointment.AppointmentPackages.Select(ap => new SelectableItem
                {
                    Id = ap.PackageId,
                    Name = ap.Package.PackageNameEn
                }).ToList(),
                Tests = appointment.AppointmentTests.Select(at => new SelectableItem
                {
                    Id = at.TestId,
                    Name = at.Test.TestNameEn
                }).ToList()
            };

            return View(model);
        }



		public async Task<JsonResult> UpdateAppointmentStatus(int AppointmentId, int StatusId, string Notes)
		{
			var appointment = await _context.Appointments.FindAsync(AppointmentId);
			if (appointment == null)
			{
				TempData["ToastAlertError"] = "الموعد غير موجود.";
				return Json(new { success = false, message = "الموعد غير موجود." });
			}

			appointment.StatusId = StatusId;
			appointment.Notes = Notes;
			await _context.SaveChangesAsync();

			TempData["ToastAlert"] = "تم تحديث حالة الموعد بنجاح.";
			return Json(new { success = true, message = "تم تحديث حالة الموعد بنجاح." });
		}



	}
}
