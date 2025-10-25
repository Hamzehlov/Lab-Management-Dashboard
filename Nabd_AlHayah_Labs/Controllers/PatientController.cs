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
                if (!string.IsNullOrEmpty(model.Password))
                {
                    model.Patient.PasswordHash = _passwordService.HashPassword(model.Password);
                }

                model.Patient.CreatedDate = DateTime.Now;
                model.Patient.IsActive = true;

                _context.Patients.Add(model.Patient);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // إعادة تعبئة قائمة الجندر في حال وجود خطأ
            model.Genders = _context.Codes
                            .Where(c => c.ParentId == 1)
                            .Select(c => new SelectListItem
                            {
                                Value = c.Id.ToString(),
                                Text = c.CodeDescEn ?? c.CodeDescEn
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

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, PatientCreateViewModel model)
        {
            if (id != model.Patient.PatientId) return BadRequest();

            if (ModelState.IsValid)
            {
                var patient = _context.Patients.FirstOrDefault(p => p.PatientId == id);
                if (patient == null) return NotFound();

                // تحديث البيانات
                patient.FullNameAr = model.Patient.FullNameAr;
                patient.FullNameEn = model.Patient.FullNameEn;
                patient.NationalId = model.Patient.NationalId;
                patient.BirthDate = model.Patient.BirthDate;
                patient.GenderId = model.Patient.GenderId;
                patient.Phone = model.Patient.Phone;
                patient.Email = model.Patient.Email;
                patient.AddressAr = model.Patient.AddressAr;
                patient.AddressEn = model.Patient.AddressEn;
                patient.GovernorateAr = model.Patient.GovernorateAr;
                patient.GovernorateEn = model.Patient.GovernorateEn;
                patient.CityAr = model.Patient.CityAr;
                patient.CityEn = model.Patient.CityEn;
                patient.BloodType = model.Patient.BloodType;
                patient.InsuranceCompanyId = model.Patient.InsuranceCompanyId;
                patient.InsuranceNumber = model.Patient.InsuranceNumber;
                patient.MoHhealthNumber = model.Patient.MoHhealthNumber;
                patient.Pat_No = model.Patient.Pat_No;
                patient.IsActive = model.Patient.IsActive;

                // تحديث الباسورد فقط إذا تم إدخاله
                if (!string.IsNullOrEmpty(model.Password))
                {
                    patient.PasswordHash = _passwordService.HashPassword(model.Password);
                }

                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // إعادة تعبئة قائمة الجندر في حال وجود خطأ
            model.Genders = _context.Codes
                          .Where(c => c.ParentId == 1)
                          .Select(c => new SelectListItem
                          {
                              Value = c.Id.ToString(),
                              Text = c.CodeDescEn ?? c.CodeDescEn
                          }).ToList();

            return View(model);
        }











    }
}
