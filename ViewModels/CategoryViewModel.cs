using MedicalExpert.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم اجباري"), StringLength(50)]
        [Display(Name = "اسم القسم")]
        public string Name { get; set; }

        [Display(Name = "الصورة")]
        public byte[] Picture { get; set; }

        public List<CheckBoxViewModel> Sides { get; set; }
        public List<CategorySide> CategoriesSides { get; set; }

        public List<CheckBoxViewModel> Diseases { get; set; }
    }
}
