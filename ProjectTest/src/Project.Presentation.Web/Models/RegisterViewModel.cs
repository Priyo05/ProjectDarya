using System.ComponentModel.DataAnnotations;

namespace Project.Presentation.Web.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FullName { get; set; } // Tambahkan properti untuk nama lengkap

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
