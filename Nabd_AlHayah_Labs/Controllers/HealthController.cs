using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MMedicalLaboratoryAPI.Data;
using Nabd_AlHayah_Labs.Model;
using Nabd_AlHayah_Labs.ViewModels;
using System;

namespace Nabd_AlHayah_Labs.Controllers
{
    public class HealthController : Controller
    {
        private readonly MedicalLaboratoryDbContext _context;

        public HealthController(MedicalLaboratoryDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> IndexTests()
        {
            var tests = await _context.Tests
                .Include(t => t.Category)
                .ToListAsync();
            return View(tests);
        }


        [HttpGet]
        public async Task<IActionResult> CreateTest()
        {
            var categories = await _context.Codes.Where(c => c.ParentId == 10).ToListAsync();
            var vm = new TestViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CodeDescEn ?? c.CodeDescEn
                })
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTest(TestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var test = new Test
                {
                    TestNameAr = model.TestNameAr,
                    TestNameEn = model.TestNameEn,
                    DescriptionAr = model.DescriptionAr,
                    DescriptionEn = model.DescriptionEn,
                    RequirementsAr = model.RequirementsAr,
                    RequirementsEn = model.RequirementsEn,
                    ShortBenefitAr = model.ShortBenefitAr,
                    ShortBenefitEn = model.ShortBenefitEn,
                    CategoryId = model.CategoryId,
                    Price = model.Price
                };

                // رفع الصورة وتحويلها إلى byte[]
                if (model.TestImageFile != null && model.TestImageFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await model.TestImageFile.CopyToAsync(ms);
                    test.TestImage = ms.ToArray();
                }

                _context.Tests.Add(test);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إضافة الفحص بنجاح.";
                return RedirectToAction(nameof(IndexTests));
            }

            // إعادة تحميل الفئات إذا فشل الـ ModelState
            model.Categories = await _context.Codes
                .Where(c => c.ParentId == 10)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CodeDescEn ?? c.CodeDescEn
                })
                .ToListAsync();

            return View(model);
        }



        public async Task<IActionResult> EditTest(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null) return NotFound();

            var categories = await _context.Codes
                .Where(c => c.ParentId == 10)
                .Select(c => new SelectListItem { Text = c.CodeDescEn , Value = c.Id.ToString() })
                .ToListAsync();

            var vm = new TestViewModel
            {
                TestId = test.TestId,
                TestNameAr = test.TestNameAr,
                TestNameEn = test.TestNameEn,
                DescriptionAr = test.DescriptionAr,
                DescriptionEn = test.DescriptionEn,
                RequirementsAr = test.RequirementsAr,
                RequirementsEn = test.RequirementsEn,
                ShortBenefitAr = test.ShortBenefitAr,
                ShortBenefitEn = test.ShortBenefitEn,
                CategoryId = test.CategoryId,
                Price = test.Price,
                TestImage = test.TestImage,
                Categories = categories
            };

            return View(vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTest(TestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var test = await _context.Tests.FindAsync(model.TestId);
                if (test == null) return NotFound();

                test.TestNameAr = model.TestNameAr;
                test.TestNameEn = model.TestNameEn;
                test.DescriptionAr = model.DescriptionAr;
                test.DescriptionEn = model.DescriptionEn;
                test.RequirementsAr = model.RequirementsAr;
                test.RequirementsEn = model.RequirementsEn;
                test.ShortBenefitAr = model.ShortBenefitAr;
                test.ShortBenefitEn = model.ShortBenefitEn;
                test.CategoryId = model.CategoryId;
                test.Price = model.Price;

                if (model.TestImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await model.TestImageFile.CopyToAsync(ms);
                    test.TestImage = ms.ToArray();
                }

                _context.Tests.Update(test);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم تعديل الفحص بنجاح.";
                return RedirectToAction(nameof(IndexTests));
            }

            model.Categories = await _context.Codes
                .Where(c => c.ParentId == 10)
                .Select(c => new SelectListItem { Text = c.CodeDescEn , Value = c.Id.ToString() })
                .ToListAsync();

            return View(model);
        }










        public async Task<IActionResult> IndexPackages()
        {
            var packages = await _context.HealthPackages
                .Include(p => p.PackageTests)
                    .ThenInclude(pt => pt.Test)
                .ToListAsync();
            return View(packages);
        }

    
        [HttpGet]
        public async Task<IActionResult> CreatePackage()
        {
            var tests = await _context.Tests
                .Include(t => t.Category)
                .ToListAsync();

            var vm = new PackageViewModel
            {
                AllTests = tests
            };

            return View(vm);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreatePackage(PackageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var package = new HealthPackage
                {
                    PackageNameAr = model.PackageNameAr,
                    PackageNameEn = model.PackageNameEn,
                    DescriptionAr = model.DescriptionAr,
                    DescriptionEn = model.DescriptionEn,
                    Price = model.Price,
                    Duration = model.Duration
                };

                // حفظ صورة الباقة
                if (model.PackageImageFile != null && model.PackageImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.PackageImageFile.CopyToAsync(ms);
                        package.PackageImage = ms.ToArray();
                    }
                }

                // ربط الفحوصات المختارة
                if (model.SelectedTestIds != null)
                {
                    foreach (var testId in model.SelectedTestIds)
                    {
                        package.PackageTests.Add(new PackageTest
                        {
                            TestId = testId
                        });
                    }
                }

                _context.HealthPackages.Add(package);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إضافة الباقة بنجاح.";
                return RedirectToAction(nameof(IndexPackages));
            }

            model.AllTests = await _context.Tests
                .Include(t => t.Category)
                .ToListAsync();

            return View(model);
        }



        // GET: Health/EditPackage/5
        public async Task<IActionResult> EditPackage(int id)
        {
            var package = await _context.HealthPackages
                .Include(p => p.PackageTests)
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package == null)
                return NotFound();

            var allTests = await _context.Tests.Include(t => t.Category).ToListAsync();

            var vm = new PackageViewModel
            {
                PackageId = package.PackageId,
                PackageNameAr = package.PackageNameAr,
                PackageNameEn = package.PackageNameEn,
                DescriptionAr = package.DescriptionAr,
                DescriptionEn = package.DescriptionEn,
                Price = package.Price,
                Duration = package.Duration,
                RequirementsAr = package.RequirementsAr,
                RequirementsEn = package.RequirementsEn,
                PackageImage = package.PackageImage,
                AllTests = allTests,
                SelectedTestIds = package.PackageTests.Select(pt => pt.TestId).ToList()
            };

            return View(vm);
        }

        // POST: Health/EditPackage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPackage(PackageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllTests = await _context.Tests.Include(t => t.Category).ToListAsync();
                return View(model);
            }

            var package = await _context.HealthPackages
                .Include(p => p.PackageTests)
                .FirstOrDefaultAsync(p => p.PackageId == model.PackageId);

            if (package == null)
                return NotFound();

            // تحديث البيانات الأساسية
            package.PackageNameAr = model.PackageNameAr;
            package.PackageNameEn = model.PackageNameEn;
            package.DescriptionAr = model.DescriptionAr;
            package.DescriptionEn = model.DescriptionEn;
            package.Price = model.Price;
            package.Duration = model.Duration;
            package.RequirementsAr = model.RequirementsAr;
            package.RequirementsEn = model.RequirementsEn;

            // تحديث الصورة إذا تم رفع صورة جديدة
            if (model.PackageImageFile != null && model.PackageImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.PackageImageFile.CopyToAsync(ms);
                package.PackageImage = ms.ToArray();
            }

            // حذف الاختبارات القديمة
            if (package.PackageTests.Any())
                _context.PackageTests.RemoveRange(package.PackageTests);

            // إضافة الاختبارات الجديدة
            if (model.SelectedTestIds != null && model.SelectedTestIds.Any())
            {
                foreach (var testId in model.SelectedTestIds)
                {
                    package.PackageTests.Add(new PackageTest
                    {
                        TestId = testId,
                        PackageId = package.PackageId
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "تم تعديل البكج بنجاح.";
            return RedirectToAction(nameof(IndexPackages));
        }




        public async Task<IActionResult> DeletePackage(int id)
        {
            var package = await _context.HealthPackages
                .Include(p => p.PackageTests)
                .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package != null)
            {
                _context.PackageTests.RemoveRange(package.PackageTests);
                _context.HealthPackages.Remove(package);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الباقة بنجاح.";
            }

            return RedirectToAction(nameof(IndexPackages));
        }

   
        public async Task<IActionResult> DeleteTest(int id)
        {
            var test = await _context.Tests
                .Include(t => t.PackageTests)
                .Include(t => t.AppointmentTests)
                .FirstOrDefaultAsync(t => t.TestId == id);

            if (test != null)
            {
                _context.PackageTests.RemoveRange(test.PackageTests);
                _context.AppointmentTests.RemoveRange(test.AppointmentTests);
                _context.Tests.Remove(test);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الفحص بنجاح.";
            }

            return RedirectToAction(nameof(IndexTests));
        }












        public async Task<IActionResult> CreatePersonalizedHealth()
        {
            var tests = await _context.Tests.ToListAsync();

            var vm = new PersonalizedHealthViewModel
            {
                AllTests = tests
            };

            return View(vm);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePersonalizedHealth(PersonalizedHealthViewModel model)
        {
            if (ModelState.IsValid)
            {
                var personalized = new PersonalizedHealth
                {
                    TestId = model.TestId,
                    CardTitleAr = model.CardTitleAr,
                    CardTitleEn = model.CardTitleEn,
                    CardSnippetAr = model.CardSnippetAr,
                    CardSnippetEn = model.CardSnippetEn,
                    TestNameAr = model.TestNameAr,
                    TestNameEn = model.TestNameEn,
                    DescriptionAr = model.DescriptionAr,
                    DescriptionEn = model.DescriptionEn,
                    RequirementsAr = model.RequirementsAr,
                    RequirementsEn = model.RequirementsEn
                };

                if (model.CardImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await model.CardImageFile.CopyToAsync(ms);
                    personalized.CardImage = ms.ToArray();
                }

                if (model.TestImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await model.TestImageFile.CopyToAsync(ms);
                    personalized.TestImage = ms.ToArray();
                }

                _context.PersonalizedHealths.Add(personalized);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إضافة بطاقة الفحص الشخصي بنجاح.";
                return RedirectToAction(nameof(IndexPersonalizedHealth));
            }

            // إعادة تحميل قائمة الفحوصات
            model.AllTests = await _context.Tests.ToListAsync();
            return View(model);
        }

        // GET: Edit
        public async Task<IActionResult> EditPersonalizedHealth(int id)
        {
            var personalized = await _context.PersonalizedHealths
                .Include(p => p.Test)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (personalized == null)
                return NotFound();

            var tests = await _context.Tests.ToListAsync();

            var vm = new PersonalizedHealthViewModel
            {
                Id = personalized.Id,
                TestId = personalized.TestId,
                CardTitleAr = personalized.CardTitleAr,
                CardTitleEn = personalized.CardTitleEn,
                CardSnippetAr = personalized.CardSnippetAr,
                CardSnippetEn = personalized.CardSnippetEn,
                TestNameAr = personalized.TestNameAr,
                TestNameEn = personalized.TestNameEn,
                DescriptionAr = personalized.DescriptionAr,
                DescriptionEn = personalized.DescriptionEn,
                RequirementsAr = personalized.RequirementsAr,
                RequirementsEn = personalized.RequirementsEn,
                CardImage = personalized.CardImage,
                TestImage = personalized.TestImage,
                AllTests = tests
            };

            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPersonalizedHealth(PersonalizedHealthViewModel model)
        {
            if (ModelState.IsValid)
            {
                var personalized = await _context.PersonalizedHealths.FindAsync(model.Id);
                if (personalized == null)
                    return NotFound();

                personalized.TestId = model.TestId;
                personalized.CardTitleAr = model.CardTitleAr;
                personalized.CardTitleEn = model.CardTitleEn;
                personalized.CardSnippetAr = model.CardSnippetAr;
                personalized.CardSnippetEn = model.CardSnippetEn;
                personalized.TestNameAr = model.TestNameAr;
                personalized.TestNameEn = model.TestNameEn;
                personalized.DescriptionAr = model.DescriptionAr;
                personalized.DescriptionEn = model.DescriptionEn;
                personalized.RequirementsAr = model.RequirementsAr;
                personalized.RequirementsEn = model.RequirementsEn;

                if (model.CardImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await model.CardImageFile.CopyToAsync(ms);
                    personalized.CardImage = ms.ToArray();
                }

                if (model.TestImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await model.TestImageFile.CopyToAsync(ms);
                    personalized.TestImage = ms.ToArray();
                }

                _context.PersonalizedHealths.Update(personalized);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم تعديل بطاقة الفحص الشخصي بنجاح.";
                return RedirectToAction(nameof(IndexPersonalizedHealth));
            }

            // إعادة تحميل قائمة الفحوصات
            model.AllTests = await _context.Tests.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> IndexPersonalizedHealth()
        {
            var list = await _context.PersonalizedHealths.Include(p => p.Test).ToListAsync();
            return View(list);
        }
    }



}

