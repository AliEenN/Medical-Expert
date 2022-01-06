using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class Disease
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int DiseaseRisk { get; set; }

        public ICollection<Medicine> Medicines { get; set; }
        public ICollection<DiseaseMedicine> DiseasesMedicines { get; set; }
    }
}
