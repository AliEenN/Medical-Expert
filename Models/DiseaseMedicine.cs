using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class DiseaseMedicine
    {
        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }

        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }
    }
}
