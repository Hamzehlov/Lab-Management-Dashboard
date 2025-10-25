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



        public IActionResult Appointments(int patientId)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == patientId);
            if (patient == null) return NotFound();

            var appointments = _context.Appointments
                                .Where(a => a.PatientId == patientId)
                                .OrderByDescending(a => a.AppointmentDate)
                                .ToList();

            var packages = _context.HealthPackages
                            .Select(p => new SelectableItem { Id = p.PackageId, Name = p.DescriptionEn })
                            .ToList();

            var tests = _context.Tests
                            .Select(t => new SelectableItem { Id = t.TestId, Name = t.TestNameEn })
                            .ToList();

            var viewModel = new AppointmentViewModel
            {
                Patient = patient,
                Appointments = appointments,
                Packages = packages,
                Tests = tests
            };

            return View(viewModel);
        }


        // POST: Add new appointment
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult AddAppointment(AppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    PatientId = model.Patient.PatientId,
                    AppointmentDate = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                // ربط الباقات والفحوصات مباشرة عبر Navigation Properties
                foreach (var pkgId in model.SelectedPackageIds)
                {
                    var package = _context.AppointmentPackages.FirstOrDefault(p => p.Id == pkgId);
                    if (package != null)
                    {
                        appointment.AppointmentPackages.Add(package);
                    }
                }

                foreach (var testId in model.SelectedTestIds)
                {
                    var test = _context.AppointmentTests.FirstOrDefault(t => t.Id == testId);
                    if (test != null)
                    {
                        appointment.AppointmentTests.Add(test);
                    }
                }

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                return RedirectToAction("Appointments", new { patientId = model.Patient.PatientId });
            }

            return View(model);
        }

        // GET: Delete appointment
        public IActionResult DeleteAppointment(int appointmentId)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == appointmentId);
            if (appointment == null) return NotFound();

            int patientId = appointment.PatientId ?? 0;
            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return RedirectToAction("Appointments", new { patientId });
        }










    }
}
