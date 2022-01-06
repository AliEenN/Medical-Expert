using MedicalExpert.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.ViewModels
{
    public class MedicineViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الدواء اجباري"), StringLength(50)]
        [Display(Name = "اسم الدواء")]
        public string Name { get; set; }

        [Required(ErrorMessage = "الجرعة اليومية اجبارية"), Range(1, 6, ErrorMessage = "يجب ان تكون القيمة من 1 الي 6")]
        [Display(Name = "الجرعة اليومية")]
        public int MedicineDose { get; set; }

        public List<CheckBoxViewModel> Diseases { get; set; }
        public ICollection<DiseaseMedicine> DiseasesMedicines { get; set; }

        [Display(Name = "نوع الدواء")]
        [Required(ErrorMessage = "نوع الدواء اجباري")]
        public int MedicineGenreId { get; set; }
        public MedicineGenre MedicineGenre { get; set; }
        public IEnumerable<RadioButtonViewModel> MedicineGenres { get; set; }


        public List<CheckBoxViewModel> MedicineForms { get; set; }
        public ICollection<MedicineAndMedicineForm> MedicinesAndMedicineForms { get; set; }
    }
}
