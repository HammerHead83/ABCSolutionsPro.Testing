using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PureMVC.Models.MsgsViewModels
{
    public class UserMessageModel
    {
        public Guid Id { get; set; }

        [Display(Name = "From")]
        public string FromUser { get; set; }

        [Display(Name = "To")]
        public string ToUser { get; set; }

        [Display(Name = "IsEmail")]
        public bool IsEmail { get; set; }

        [Display(Name = "Body")]
        public string Body { get; set; }
    }
}
