using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MMedicalLaboratoryAPI.Data;
using Nabd_AlHayah_Labs.Models;
using Nabd_AlHayah_Labs.ViewModels;

namespace Nabd_AlHayah_Labs.Controllers
{

    public class AccountController : Controller
    {

        private readonly MedicalLaboratoryDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AccountController(MedicalLaboratoryDbContext context , UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
             _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // جلب المستخدم من قاعدة البيانات
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();

            // جلب الدور (الاسم فقط)
            var roles = await _userManager.GetRolesAsync(user);
            string roleName = roles.FirstOrDefault() ?? "No Role";

            // تجهيز ViewModel
            var viewModel = new UserDetailsViewModel
            {
                User = user,
                RoleName = roleName,
                UserLogs = null // بما انه ما عندك جدول Logs
            };

            return View(viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new UserCreateViewModel
            {
                Roles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                    .ToListAsync();

                return View(model);
            }

            var user = new ApplicationUser
            {
                FullName = model.FirstName,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = model.EmailConfirmed,
                BranchID = 1

            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                TempData["Success"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            model.Roles = await _roleManager.Roles
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToListAsync();

            return View(model);
        }
        

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                Roles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                    .ToListAsync()
            };

            var roles = await _userManager.GetRolesAsync(user);
            model.Role = roles.FirstOrDefault() ?? string.Empty;

            return View(model);
        }

        [HttpPost]
		public async Task<IActionResult> Edit(EditUserViewModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Roles = await _roleManager.Roles
					.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
					.ToListAsync();
				return View(model);
			}

			var user = await _userManager.FindByIdAsync(model.Id);
			if (user == null) return NotFound();

			user.FullName = model.FirstName;
			user.Email = model.Email;
			user.UserName = model.Email;
			user.PhoneNumber = model.PhoneNumber;
			user.EmailConfirmed = model.EmailConfirmed;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					ModelState.AddModelError("", error.Description);
				model.Roles = await _roleManager.Roles
					.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
					.ToListAsync();
				return View(model);
			}

			// تعديل كلمة السر فقط إذا تم إدخالها
			if (!string.IsNullOrEmpty(model.NewPassword))
			{
				if (model.NewPassword != model.ConfirmPassword)
				{
					ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
					model.Roles = await _roleManager.Roles
						.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
						.ToListAsync();
					return View(model);
				}

				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var passResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
				if (!passResult.Succeeded)
				{
					foreach (var error in passResult.Errors)
						ModelState.AddModelError("", error.Description);
					model.Roles = await _roleManager.Roles
						.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
						.ToListAsync();
					return View(model);
				}
			}

			// تحديث الدور
			var currentRoles = await _userManager.GetRolesAsync(user);
			if (!currentRoles.Contains(model.Role))
			{
				await _userManager.RemoveFromRolesAsync(user, currentRoles);
				await _userManager.AddToRoleAsync(user, model.Role);
			}

			TempData["ToastAlert"] = "User updated successfully!";
			return RedirectToAction(nameof(Index));
		}


		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(string email, string password, bool rememberMe)
		{
			var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);

			if (result.Succeeded)
				return RedirectToAction("Index", "Home");

			if (result.IsLockedOut)
				return View("Lockout");

			ViewBag.Error = "Invalid email or password.";

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}



	}
}
