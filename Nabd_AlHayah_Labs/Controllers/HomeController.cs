using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMedicalLaboratoryAPI.Data;
using Nabd_AlHayah_Labs.Model;
using Nabd_AlHayah_Labs.Models;
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
            return View();
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

		// ??? ????????
		public async Task<IActionResult> DetailsNews(int id)
		{
			var news = await _context.NewsEvents.FindAsync(id);
			if (news == null)
				return NotFound();

			return View(news);
		}

		// ??? ???? ???????
		public IActionResult CreateNews()
		{
			return View();
		}

		// ??? ???????? ??? ???????
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateNews(NewsEvent model, IFormFile? ImageFile)
		{
			if (ModelState.IsValid)
			{
				if (ImageFile != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
					string filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await ImageFile.CopyToAsync(stream);
					}
					model.ImageUrl = "/uploads/" + fileName;
				}

				_context.Add(model);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(model);
		}

		// ??? ???? ???????
		public async Task<IActionResult> EditNews(int id)
		{
			var news = await _context.NewsEvents.FindAsync(id);
			if (news == null)
				return NotFound();

			return View(news);
		}

		// ??? ?????????
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditNews(int id, NewsEvent model, IFormFile? ImageFile)
		{
			if (id != model.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					var existing = await _context.NewsEvents.FindAsync(id);
					if (existing == null)
						return NotFound();

					existing.TitleAr = model.TitleAr;
					existing.TitleEn = model.TitleEn;
					existing.DescriptionAr = model.DescriptionAr;
					existing.DescriptionEn = model.DescriptionEn;
					existing.EventDate = model.EventDate;

					if (ImageFile != null)
					{
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
						string filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await ImageFile.CopyToAsync(stream);
						}
						existing.ImageUrl = "/uploads/" + fileName;
					}

					_context.Update(existing);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				catch (DbUpdateConcurrencyException)
				{
					return NotFound();
				}
			}
			return View(model);
		}

		// ????? ?????
		public async Task<IActionResult> Delete(int id)
		{
			var news = await _context.NewsEvents.FindAsync(id);
			if (news == null)
				return NotFound();

			return View(news);
		}

		// ????? ?????
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var news = await _context.NewsEvents.FindAsync(id);
			if (news != null)
			{
				if (!string.IsNullOrEmpty(news.ImageUrl))
				{
					var path = Path.Combine(_env.WebRootPath, news.ImageUrl.TrimStart('/'));
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
				}

				_context.NewsEvents.Remove(news);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}





		//_________________________________________________




		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
