using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMedicalLaboratoryAPI.Data;
using Nabd_AlHayah_Labs.ViewModels;
using System.Net.Http;
using System.Text.Json;

namespace Nabd_AlHayah_Labs.Controllers
{
    public class ResultsAndInvoicesController : Controller
    {
        private readonly MedicalLaboratoryDbContext _context;
        private readonly IWebHostEnvironment _env;

        private readonly HttpClient _httpClient;
        private readonly string _labBaseUrl;

        public ResultsAndInvoicesController(ILogger<HomeController> logger, MedicalLaboratoryDbContext context, IWebHostEnvironment env , HttpClient httpClient, IConfiguration configuration)

        {

            _context = context;

            _httpClient = httpClient;
            _labBaseUrl = configuration["ExternalApis:LabBaseUrl"];
            _env = env;

        }



        public async Task<IActionResult> Invoices(int patientId)
        {
            if (patientId <= 0)
                return BadRequest("PatientId is required.");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
            if (patient == null || patient.Pat_No == null)
                return NotFound("Patient not found or missing Pat_No.");

            var externalApiUrl = $"{_labBaseUrl}api/Lab/GetPatientInvoice?Pat_No={patient.Pat_No}";

            try
            {
                var response = await _httpClient.GetAsync(externalApiUrl);
                if (!response.IsSuccessStatusCode)
                    return View("Error", $"Failed to fetch invoices from external API. Status: {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                var invoices = JsonSerializer.Deserialize<List<InvoiceViewModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.PatientName = patient.FullNameEn; // أو أي حقل للعرض
                return View(invoices);
            }
            catch
            {
                return View("Error", "An error occurred while fetching invoices.");
            }
        }

        // 🔹 View للنتائج المخبرية
        public async Task<IActionResult> Results(int patientId)
        {
            if (patientId <= 0)
                return BadRequest("PatientId is required.");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
            if (patient == null || patient.Pat_No == null)
                return NotFound("Patient not found or missing Pat_No.");

            var externalApiUrl = $"{_labBaseUrl}api/Lab/GetPatientResults?Pat_No={patient.Pat_No}";

            try
            {
                var response = await _httpClient.GetAsync(externalApiUrl);
                if (!response.IsSuccessStatusCode)
                    return View("Error", $"Failed to fetch lab results from external API. Status: {response.StatusCode}");

                var json = await response.Content.ReadAsStringAsync();
                var results = JsonSerializer.Deserialize<List<LabResultViewModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // تعديل رابط التحميل
                results.ForEach(r =>
                {
                    if (!string.IsNullOrEmpty(r.ResultLink))
                        r.ResultLink = $"{_labBaseUrl.TrimEnd('/')}/{r.ResultLink.TrimStart('/')}";
                });

                ViewBag.PatientName = patient.FullNameEn;
                return View(results);
            }
            catch
            {
                return View("Error", "An error occurred while fetching lab results.");
            }
        }

        // 🔹 تحميل ملف النتيجة PDF
        public IActionResult DownloadResult(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return BadRequest("File URL is required.");

            return Redirect(fileUrl);
        }
    }
}
