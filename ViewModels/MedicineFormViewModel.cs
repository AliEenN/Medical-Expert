using MedicalExpert.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.ViewModels
{
    public class MedicineFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الشكل اجباري"), StringLength(50)]
        [Display(Name = "اسم الشكل")]
        public string Name { get; set; }

        public List<CheckBoxViewModel> Medicines { get; set; }
        public ICollection<MedicineAndMedicineForm> MedicinesAndMedicineForms { get; set; }
    }
}
