using MedicalExpert.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.ViewModels
{
    public class MedicineGenreViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نوع الدواء اجباري"), StringLength(50)]
        [Display(Name = "نوع الدواء")]
        public string Name { get; set; }

        public IEnumerable<Medicine> Medicines { get; set; }
    }
}
