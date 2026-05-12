using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace AdoptMeNow.ViewModels
{
    public class RegisterViewModel
    {
     [Required(ErrorMessage = "Full Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required. ")]
        [StringLength(40,MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Passwords do not match.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required. ")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}