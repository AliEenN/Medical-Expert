using MedicalExpert.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.ViewModels
{
    public class DiseaseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المرض اجباري"), StringLength(50)]
        [Display(Name = "اسم المرض")]
        public string Name { get; set; }

        [Required(ErrorMessage = "يجب اختيار قسم")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public IEnumerable<RadioButtonViewModel> Categories { get; set; }

        public List<CheckBoxViewModel> Medicines { get; set; }
        public ICollection<DiseaseMedicine> DiseasesMedicines { get; set; }
        public int DiseaseRisk { get; set; }
        public IEnumerable<RadioButtonViewModel> DiseaseRisks { get; set; }
    }
}
