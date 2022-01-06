using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class Medicine
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, Range(1, 6)]
        public int MedicineDose { get; set; }

        public ICollection<Disease> Diseases { get; set; }
        public ICollection<DiseaseMedicine> DiseasesMedicines { get; set; }

        public int MedicineGenreId { get; set; }
        public MedicineGenre MedicineGenre { get; set; }

        public ICollection<MedicineForm> MedicineForms { get; set; }
        public ICollection<MedicineAndMedicineForm> MedicinesAndMedicineForms { get; set; }
    }
}
