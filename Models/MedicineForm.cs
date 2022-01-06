using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class MedicineForm
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        public ICollection<Medicine> Medicines { get; set; }
        public ICollection<MedicineAndMedicineForm> MedicinesAndMedicineForms { get; set; }
    }
}
