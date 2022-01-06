using MedicalExpert.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.ViewModels
{
    public class SideViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "وصف العرص اجباري"), StringLength(250)]
        [Display(Name = "وصف العرض")]
        public string Description { get; set; }

        public List<CheckBoxViewModel> Categories { get; set; }
        public ICollection<CategorySide> CategoriesSides { get; set; }
    }
}
