using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMedicalLaboratoryAPI.Data;

namespace Nabd_AlHayah_Labs.Controllers
{
	public class PatientController : Controller
	{


		private readonly MedicalLaboratoryDbContext _context;



		public PatientController(MedicalLaboratoryDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			var patients = await _context.Patients
				.Include(p => p.Gender)
				.ToListAsync();
			return View(patients);
		}








	}
}
