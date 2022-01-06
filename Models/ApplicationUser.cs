using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "الاسم الاول اجباري"), MaxLength(25)]
        [Display(Name = "الاسم الاول")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "الاسم الاخير اجباري"), MaxLength(25)]
        [Display(Name = "الاسم الاخير")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "السن اجباري")]
        [Display(Name = "السن")]
        public int Age { get; set; }

        [Display(Name = "الصورة")]
        public byte[] Image { get; set; }
    }
}
