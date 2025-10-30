using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMedicalLaboratoryAPI.Data;
using Nabd_AlHayah_Labs.Model;
using Nabd_AlHayah_Labs.Models;
using Nabd_AlHayah_Labs.ViewModels;
using System.Diagnostics;

namespace Nabd_AlHayah_Labs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


		private readonly MedicalLaboratoryDbContext _context;
		private readonly IWebHostEnvironment _env;

		public HomeController(ILogger<HomeController> logger , MedicalLaboratoryDbContext context, IWebHostEnvironment env)  

        {

			_context = context;
			_logger = logger;
			_env = env;

		}

        public IActionResult Index()
        {
            // إجمالي المرضى
            int totalPatients = _context.Patients.Count();

            // إجمالي المواعيد
            int totalAppointments = _context.Appointments.Where(x=>x.StatusId==7).Count();

            // المرضى الذين لديهم رقم ملف (Pat_No موجود)
            int patientsWithFileNumber = _context.Patients.Count(p => p.Pat_No != null);

            // المرضى الذين لا يملكون رقم ملف (Pat_No غير موجود)
            int patientsWithoutFileNumber = _context.Patients.Count(p => p.Pat_No == null);

            var model = new DashboardViewModel
            {
                TotalPatients = totalPatients,
                TotalAppointments = totalAppointments,
                ApprovedPatients = patientsWithFileNumber,
                PendingPatients = patientsWithoutFileNumber
            };

            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

		//____________________________________________________


		public async Task<IActionResult> Indexnews()
		{
			var news = await _context.NewsEvents.ToListAsync();
			return View(news);
		}

	
		public async Task<IActionResult> DetailsNews(int id)
		{
			var news = await _context.NewsEvents.FindAsync(id);
			if (news == null)
				return NotFound();

			return View(news);
		}

		public IActionResult CreateNews()
		{
            var model = new NewsEvent
            {
                IsActive = true // أو false حسب ما تريد كافتراضي
            };
            return View(model);
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNews(NewsEvent model, IFormFile? ImageFile)
		{
			if (ModelState.IsValid)
			{
				if (ImageFile != null && ImageFile.Length > 0)
				{
					using (var ms = new MemoryStream())
					{
						await ImageFile.CopyToAsync(ms);
						model.Image = ms.ToArray(); // حفظ الصورة بصيغة باينري
					}
				}

				_context.Add(model);
				await _context.SaveChangesAsync();

				TempData["ToastAlert"] = "تم إضافة الخبر بنجاح. | News added successfully.";
				return RedirectToAction(nameof(Indexnews));
			}

			TempData["ToastAlertError"] = "يرجى التحقق من البيانات المدخلة. | Please check the entered data.";
			return View(model);
		}


		public async Task<IActionResult> EditNews(int id)
		{
			var news = await _context.NewsEvents.FindAsync(id);
			if (news == null)
				return NotFound();

			return View(news);
		}

	
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditNews(int id, NewsEvent model, IFormFile? ImageFile)
		{
			if (id != model.Id)
			{
				TempData["ToastAlertError"] = "المعرف غير صحيح. | Invalid ID.";
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					var existing = await _context.NewsEvents.FindAsync(id);
					if (existing == null)
					{
						TempData["ToastAlertError"] = "الخبر غير موجود. | News not found.";
						return NotFound();
					}

					// تحديث البيانات
					existing.TitleAr = model.TitleAr;
					existing.TitleEn = model.TitleEn;
					existing.DescriptionAr = model.DescriptionAr;
					existing.DescriptionEn = model.DescriptionEn;
					existing.EventDate = model.EventDate;
				   existing.IsActive= model.IsActive;

					// تحديث الصورة إذا تم اختيار صورة جديدة
					if (ImageFile != null && ImageFile.Length > 0)
					{
						using (var ms = new MemoryStream())
						{
							await ImageFile.CopyToAsync(ms);
							existing.Image = ms.ToArray();
						}
					}

					_context.Update(existing);
					await _context.SaveChangesAsync();

					TempData["ToastAlert"] = "تم تعديل الخبر بنجاح. | News updated successfully.";
					return RedirectToAction(nameof(Indexnews));
				}
				catch (DbUpdateConcurrencyException)
				{
					TempData["ToastAlertError"] = "حدث خطأ أثناء تعديل الخبر. | Error updating news.";
					return NotFound();
				}
			}

			TempData["ToastAlertError"] = "يرجى التحقق من البيانات المدخلة. | Please check the entered data.";
			return View(model);
		}




		[HttpPost]

		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var news = await _context.NewsEvents.FindAsync(id);
			if (news == null)
			{
				TempData["ToastAlertError"] = "الخبر غير موجود. | News not found.";
				return RedirectToAction(nameof(Indexnews));
			}

			try
			{
				if (!string.IsNullOrEmpty(news.ImageUrl))
				{
					var path = Path.Combine(_env.WebRootPath, news.ImageUrl.TrimStart('/'));
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
				}

				_context.NewsEvents.Remove(news);
				await _context.SaveChangesAsync();

				TempData["ToastAlert"] = "تم حذف الخبر بنجاح. | News deleted successfully.";
			}
			catch (Exception ex)
			{
				TempData["ToastAlertError"] = $"حدث خطأ أثناء حذف الخبر: {ex.Message} | Error deleting news: {ex.Message}";
			}

			return RedirectToAction(nameof(Indexnews));
		}





		//_________________________________________________




		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
