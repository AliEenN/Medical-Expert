using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.ViewModels
{
    public class AdviceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان النصيحة مطلوب"), StringLength(75)]
        [Display(Name = "عنوان النصيحة")]
        public string Title { get; set; }

        [Required(ErrorMessage = "شرح النصيحة مطلوب"), StringLength(250)]
        [Display(Name = "شرح النصيحة")]
        public string Description { get; set; }

        [Display(Name = "الصورة")]
        public byte[] Picture { get; set; }
    }
}
