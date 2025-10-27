using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MMedicalLaboratoryAPI.Data;
using Nabd_AlHayah_Labs.Model;
using Nabd_AlHayah_Labs.Service;
using Nabd_AlHayah_Labs.ViewModels;
using System.Text;

namespace Nabd_AlHayah_Labs.Controllers
{
	public class PatientController : Controller
	{


		private readonly MedicalLaboratoryDbContext _context;
        private readonly IPasswordService _passwordService;

        public PatientController(MedicalLaboratoryDbContext context , IPasswordService passwordService)
		{
			_context = context;
            _passwordService = passwordService;
        }
        public async Task<IActionResult> Index()
        {
            var patients = await _context.Patients
                .Include(p => p.Gender)
                .ToListAsync();
            return View(patients);
        }


        public async Task<IActionResult> Details(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.HealthMonitorings)
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            var viewModel = new PatientDetailsViewModel
            {
                Patient = patient,
                HealthMonitorings = patient.HealthMonitorings,
                Appointments = patient.Appointments
            };

            return View(viewModel);
        }




        public IActionResult Create()
        {
            var viewModel = new PatientCreateViewModel
            {
                Genders = _context.Codes
                            .Where(c => c.ParentId == 1)
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.CodeDescEn ?? c.CodeDescEn
                            }).ToList(),
            };

            return View(viewModel);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PatientCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ✅ التحقق من التكرار في National ID
                if (!string.IsNullOrEmpty(model.Patient.NationalId) &&
                    _context.Patients.Any(p => p.NationalId == model.Patient.NationalId))
                {
                    ModelState.AddModelError("Patient.NationalId", "National ID already exists.");
                }

                // ✅ التحقق من التكرار في Phone
                if (!string.IsNullOrEmpty(model.Patient.Phone) &&
                    _context.Patients.Any(p => p.Phone == model.Patient.Phone))
                {
                    ModelState.AddModelError("Patient.Phone", "Phone number already exists.");
                }

                // ✅ التحقق من التكرار في Email
                if (!string.IsNullOrEmpty(model.Patient.Email) &&
                    _context.Patients.Any(p => p.Email == model.Patient.Email))
                {
                    ModelState.AddModelError("Patient.Email", "Email already exists.");
                }

                // ✅ التحقق من التكرار في Pat_No
                if (model.Patient.Pat_No != null &&
                    _context.Patients.Any(p => p.Pat_No == model.Patient.Pat_No))
                {
                    ModelState.AddModelError("Patient.Pat_No", "Patient number already exists.");
                }

                // إذا فيه أي أخطاء تحقق نرجع الصفحة نفسها
                if (!ModelState.IsValid)
                {
                    model.Genders = _context.Codes
                        .Where(c => c.ParentId == 1)
                        .Select(c => new SelectListItem
                        {
                            Value = c.Id.ToString(),
                            Text = c.CodeDescEn
                        }).ToList();

                    return View(model);
                }

                // ✅ كل شيء تمام — نكمل عملية الإضافة
                if (!string.IsNullOrEmpty(model.Password))
                {
                    model.Patient.PasswordHash = _passwordService.HashPassword(model.Password);
                }

                model.Patient.CreatedDate = DateTime.Now;
                model.Patient.IsActive = true;

                _context.Patients.Add(model.Patient);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Patient added successfully!";
                return RedirectToAction("Index");
            }

            // إعادة تعبئة قائمة الجندر في حال فشل التحقق العام
            model.Genders = _context.Codes
                .Where(c => c.ParentId == 1)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CodeDescEn
                }).ToList();

            return View(model);
        }





        // GET: Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == id);
            if (patient == null) return NotFound();

            var viewModel = new PatientCreateViewModel
            {
                Patient = patient,
                Genders = _context.Codes
                            .Where(c => c.ParentId == 1)
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.CodeDescEn ?? c.CodeDescEn
                            }).ToList(),
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PatientCreateViewModel model)
        {
            if (id != model.Patient.PatientId)
                return BadRequest();

            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == id);
            if (patient == null)
                return NotFound();

            // Duplicate checks excluding current record
            if (_context.Patients.Any(p => p.NationalId == model.Patient.NationalId && p.PatientId != id))
                ModelState.AddModelError("Patient.NationalId", "National ID already exists.");

            if (!string.IsNullOrEmpty(model.Patient.Phone) && _context.Patients.Any(p => p.Phone == model.Patient.Phone && p.PatientId != id))
                ModelState.AddModelError("Patient.Phone", "Phone number already exists.");

            if (!string.IsNullOrEmpty(model.Patient.Email) && _context.Patients.Any(p => p.Email == model.Patient.Email && p.PatientId != id))
                ModelState.AddModelError("Patient.Email", "Email already exists.");

            if (model.Patient.Pat_No.HasValue &&
                  _context.Patients.Any(p => p.Pat_No == model.Patient.Pat_No && p.PatientId != id))
             {
                ModelState.AddModelError("Patient.Pat_No", "Patient number already exists.");
            }


            if (ModelState.IsValid)
            {
                model.Genders = _context.Codes
                                .Where(c => c.ParentId == 1)
                                .Select(c => new SelectListItem
                                {
                                    Value = c.Id.ToString(),
                                    Text = c.CodeDescEn ?? c.CodeDescEn
                                }).ToList();
                return View(model);
            }

            // Update patient
            patient.FullNameAr = model.Patient.FullNameAr;
            patient.FullNameEn = model.Patient.FullNameEn;
            patient.NationalId = model.Patient.NationalId;
            patient.BirthDate = model.Patient.BirthDate;
            patient.GenderId = model.Patient.GenderId;
            patient.Phone = model.Patient.Phone;
            patient.Email = model.Patient.Email;
            patient.AddressAr = model.Patient.AddressAr;
            patient.AddressEn = model.Patient.AddressEn;
            patient.CityAr = model.Patient.CityAr;
            patient.CityEn = model.Patient.CityEn;
            patient.Pat_No = model.Patient.Pat_No;
            patient.IsActive = model.Patient.IsActive;

            if (!string.IsNullOrEmpty(model.Password))
            {
                patient.PasswordHash = _passwordService.HashPassword(model.Password);
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Patient updated successfully!";
            return RedirectToAction("Index");
        }



        public IActionResult HealthMonitoring(int patientId)
        {
            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientId == patientId);
            if (patient == null) return NotFound();

            var records = _context.HealthMonitorings
                .Where(h => h.PatientId == patientId)
                .OrderByDescending(h => h.CreatedAt)
                .ToList();

            var model = new HealthMonitoringViewModel
            {
                Patient = patient,
                HealthMonitorings = records
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddHealthMonitoring(HealthMonitoring model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                _context.HealthMonitorings.Add(model);
                _context.SaveChanges();
            }
            return RedirectToAction("HealthMonitoring", new { patientId = model.PatientId });
        }

        public IActionResult EditHealthMonitoring(int monitorId)
        {
            var record = _context.HealthMonitorings.FirstOrDefault(h => h.MonitorId == monitorId);
            if (record == null) return NotFound();
            return View(record);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditHealthMonitoring(HealthMonitoring model)
        {
            if (ModelState.IsValid)
            {
                _context.HealthMonitorings.Update(model);
                _context.SaveChanges();
            }
            return RedirectToAction("HealthMonitoring", new { patientId = model.PatientId });
        }

        public IActionResult DeleteHealthMonitoring(int monitorId)
        {
            var record = _context.HealthMonitorings.FirstOrDefault(h => h.MonitorId == monitorId);
            if (record == null) return NotFound();
            int patientId = record.PatientId ?? 0;
            _context.HealthMonitorings.Remove(record);
            _context.SaveChanges();
            return RedirectToAction("HealthMonitoring", new { patientId });
        }



 
        public async Task<IActionResult> Appointments(int patientId)
        {
            // 🧾 جلب بيانات المريض
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null)
                return NotFound("المريض غير موجود.");

            // 📅 جلب المواعيد المرتبطة بالمريض
            var appointments = await _context.Appointments
                .Include(a => a.AppointmentType)
                .Include(a => a.AppointmentTests).ThenInclude(t => t.Test)
                .Include(a => a.AppointmentPackages).ThenInclude(p => p.Package)
                .Include(a => a.HomeSamplings)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            // 🧪 الفحوصات المتاحة
            var tests = await _context.Tests
                .Select(t => new SelectableItem { Id = t.TestId, Name = t.TestNameEn })
                .ToListAsync();

            // 🎁 الباقات المتاحة
            var packages = await _context.HealthPackages
                .Select(p => new SelectableItem { Id = p.PackageId, Name = p.PackageNameEn })
                .ToListAsync();

            // 🔸 أنواع المواعيد
            var appointmentTypes = await _context.Codes
                .Where(c => c.ParentId==8)
                .Select(c => new SelectableItem
                {
                    Id = c.Id,
                    Name = $"{c.CodeDescEn} / {c.CodeDescAr}"
                })
                .ToListAsync();

            // 📦 تعبئة الـ ViewModel
            var viewModel = new AppointmentViewModel
            {
                Patient = patient,
                Appointments = appointments,
                Tests = tests,
                Packages = packages,
                AppointmentTypes = appointmentTypes
            };

            return View(viewModel);
        }


        // ==================================
        // 🔹 إنشاء موعد جديد (إنشاء + فحوصات + باقات + زيارة منزلية)
        // ==================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Error"] = "يرجى التحقق من البيانات المدخلة.";
                return RedirectToAction("Appointment", new { patientId = model.Patient.PatientId });
            }

            try
            {
                // 🗓️ إنشاء الموعد الأساسي
                var appointment = new Appointment
                {
                    PatientId = model.Patient.PatientId,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTypeId = model.AppointmentTypeId,
                    Notes = model.Notes,
                    CreatedAt = DateTime.Now,
                    StatusId = 5
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // 🏠 في حال الموعد منزلي (ID = 10)
                var isHomeSampling = model.AppointmentTypeId == 10;
                if (isHomeSampling)
                {
                    var homeSampling = new HomeSampling
                    {
                        AppointmentId = appointment.AppointmentId,
                        AddressAr = model.AddressAr ?? "غير محدد",
                        AddressEn = model.AddressEn,
                        CityAr = model.CityAr,
                        CityEn = model.CityEn,
                        TechnicianName = model.TechnicianName,
                        VisitTime = model.AppointmentDate,
                        IsForAnotherPerson = model.IsForAnotherPerson
                    };

                    _context.HomeSamplings.Add(homeSampling);
                }

                // 🧪 إضافة الفحوصات
                foreach (var testId in model.SelectedTestIds)
                {
                    _context.AppointmentTests.Add(new AppointmentTest
                    {
                        AppointmentId = appointment.AppointmentId,
                        TestId = testId
                    });
                }

                // 🎁 إضافة الباقات
                foreach (var packageId in model.SelectedPackageIds)
                {
                    _context.AppointmentPackages.Add(new AppointmentPackage
                    {
                        AppointmentId = appointment.AppointmentId,
                        PackageId = packageId
                    });
                }

                await _context.SaveChangesAsync();

                // ✅ إشعار النجاح
                TempData["Success"] = isHomeSampling
                    ? "تم حجز الموعد المنزلي بنجاح."
                    : "تم حجز الموعد في المختبر بنجاح.";

                return RedirectToAction("Appointment", new { patientId = model.Patient.PatientId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء إنشاء الموعد: {ex.Message}";
                return RedirectToAction("Appointment", new { patientId = model.Patient.PatientId });
            }
        }


        // ==================================
        // 🔹 تعديل موعد قائم
        // ==================================
        // GET: Load appointment for editing
        public async Task<IActionResult> EditAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.AppointmentTests)
                .Include(a => a.AppointmentPackages)
                .Include(a => a.HomeSamplings)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
            {
                TempData["Error"] = "الموعد غير موجود.";
                return RedirectToAction("Index", new { patientId = 0 }); // أو إعادة التوجيه للصفحة المناسبة
            }

            // جلب الفحوصات والباقات وأنواع الموعد
            var tests = await _context.Tests
                .Select(t => new SelectableItem { Id = t.TestId, Name = t.TestNameEn })
                .ToListAsync();

            var packages = await _context.HealthPackages
                .Select(p => new SelectableItem { Id = p.PackageId, Name = p.PackageNameEn })
                .ToListAsync();

            var appointmentTypes = await _context.Codes
                .Where(c => c.ParentId == 8)
                .Select(c => new SelectableItem { Id = c.Id, Name = c.CodeDescEn + " / " + c.CodeDescAr })
                .ToListAsync();

            // تعبئة ViewModel
            var model = new AppointmentViewModel
            {
                AppointmentId = appointment.AppointmentId,
                Patient = appointment.Patient,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTypeId = (int)appointment.AppointmentTypeId,
                Notes = appointment.Notes,
                SelectedTestIds = appointment.AppointmentTests.Select(t => t.TestId).ToList(),
                SelectedPackageIds = appointment.AppointmentPackages.Select(p => p.PackageId).ToList(),
                Tests = tests,
                Packages = packages,
                AppointmentTypes = appointmentTypes
            };

            // تعبئة حقول السحب المنزلي إذا موجود
            var home = appointment.HomeSamplings.FirstOrDefault();
            if (home != null)
            {
                model.AddressAr = home.AddressAr;
                model.AddressEn = home.AddressEn;
                model.CityAr = home.CityAr;
                model.CityEn = home.CityEn;
                model.TechnicianName = home.TechnicianName;
                model.IsForAnotherPerson =  (bool) home.IsForAnotherPerson;
            }

            return View(model);
        }





        // POST: Edit appointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAppointment(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                TempData["Error"] = "يرجى التحقق من البيانات.";
                return RedirectToAction("EditAppointment", new { id = model.AppointmentId });
            }

            var appointment = await _context.Appointments
                .Include(a => a.AppointmentTests)
                .Include(a => a.AppointmentPackages)
                .Include(a => a.HomeSamplings)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == model.AppointmentId);

            if (appointment == null)
            {
                TempData["Error"] = "الموعد غير موجود.";
                return RedirectToAction("Index", new { patientId = model.Patient.PatientId });
            }

            try
            {
                // تحديث بيانات الموعد
                appointment.AppointmentDate = model.AppointmentDate;
                appointment.AppointmentTypeId = model.AppointmentTypeId;
                appointment.Notes = model.Notes;
                appointment.StatusId = 5;
                // حذف القديم من الفحوصات والباقات
                _context.AppointmentTests.RemoveRange(appointment.AppointmentTests);
                _context.AppointmentPackages.RemoveRange(appointment.AppointmentPackages);

                // إضافة الجديد
                foreach (var testId in model.SelectedTestIds)
                    _context.AppointmentTests.Add(new AppointmentTest { AppointmentId = appointment.AppointmentId, TestId = testId });

                foreach (var packageId in model.SelectedPackageIds)
                    _context.AppointmentPackages.Add(new AppointmentPackage { AppointmentId = appointment.AppointmentId, PackageId = packageId });

                // 🏠 التحقق من الموعد المنزلي
                var isHomeSampling = appointment.AppointmentTypeId == 10;
                var homeSampling = await _context.HomeSamplings
                    .FirstOrDefaultAsync(h => h.AppointmentId == appointment.AppointmentId);

                if (isHomeSampling)
                {
                    if (homeSampling == null)
                    {
                        _context.HomeSamplings.Add(new HomeSampling
                        {
                            AppointmentId = appointment.AppointmentId,
                            AddressAr = model.AddressAr,
                            AddressEn = model.AddressEn,
                            CityAr = model.CityAr,
                            CityEn = model.CityEn,
                            TechnicianName = model.TechnicianName,
                            VisitTime = model.AppointmentDate,
                            IsForAnotherPerson = model.IsForAnotherPerson
                        });
                    }
                    else
                    {
                        homeSampling.AddressAr = model.AddressAr;
                        homeSampling.AddressEn = model.AddressEn;
                        homeSampling.CityAr = model.CityAr;
                        homeSampling.CityEn = model.CityEn;
                        homeSampling.TechnicianName = model.TechnicianName;
                        homeSampling.VisitTime = model.AppointmentDate;
                        homeSampling.IsForAnotherPerson = model.IsForAnotherPerson;
                    }
                }
                else
                {
                    // حذف الموعد المنزلي إن وجد
                    if (homeSampling != null)
                        _context.HomeSamplings.Remove(homeSampling);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "تم تعديل الموعد بنجاح.";
                return RedirectToAction("Appointments", new { patientId = model.Patient.PatientId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء تعديل الموعد: {ex.Message}";
                return RedirectToAction("EditAppointment", new { id = model.AppointmentId });
            }
        }



        // ==================================
        // 🔹 حذف موعد
        // ==================================
        public async Task<IActionResult> DeleteAppointment(int appointmentId, int patientId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment == null)
            {
                TempData["Error"] = "الموعد غير موجود.";
                return RedirectToAction("Appointment", new { patientId });
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "تم حذف الموعد بنجاح.";
            return RedirectToAction("Appointment", new { patientId });
        }




        // GET: عرض تفاصيل الموعد
        public async Task<IActionResult> AppointmentDetails(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.AppointmentType) // نوع الموعد
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
                Patient = appointment.Patient,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTypeId =(int) appointment.AppointmentTypeId,
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





    }
}
