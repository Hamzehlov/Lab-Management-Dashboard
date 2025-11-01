using Microsoft.AspNetCore.Mvc.Rendering;
using Nabd_AlHayah_Labs.Models;
using System.ComponentModel.DataAnnotations;

namespace Nabd_AlHayah_Labs.ViewModels
{
    public class UserViewModel
    {
    }


    public class UserCreateViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

      

        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();
    }

	public class EditUserViewModel
	{
		public string Id { get; set; } = string.Empty;

		[Required]
		[Display(Name = "Full Name")]
		public string FirstName { get; set; } = string.Empty;

		[Required, EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; } = string.Empty;

		[Display(Name = "Phone Number")]
		public string PhoneNumber { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string? NewPassword { get; set; }  // nullable

		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		public string? ConfirmPassword { get; set; }  // nullable

		[Required]
		[Display(Name = "Role")]
		public string Role { get; set; } = string.Empty;

		public bool EmailConfirmed { get; set; }

		public List<SelectListItem> Roles { get; set; } = new();
	}

	public class UserDetailsViewModel
    {
        public ApplicationUser User { get; set; }
        public string RoleName { get; set; }

   
        public List<UserLogViewModel> UserLogs { get; set; }
    }

    public class UserLogViewModel
    {
        public string ActionType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }



}
