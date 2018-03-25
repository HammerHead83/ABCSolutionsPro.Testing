using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PureMVC.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        public string MidName { get; set; }

        [Display(Name = "Администратор")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Заблокирован")]
        public bool IsLocked { get; set; }

        public string StatusMessage { get; set; }
    }
}
