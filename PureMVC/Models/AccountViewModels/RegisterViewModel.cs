using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PureMVC.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Text)]
        [StringLength(25, ErrorMessage = "First name lehgth cann't be greater then 25 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(25, ErrorMessage = "Last name lehgth cann't be greater then 25 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(25, ErrorMessage = "Middle name lehgth cann't be greater then 25 characters.")]
        [Display(Name = "Middle Name")]
        public string MidName { get; set; }
    }
}
