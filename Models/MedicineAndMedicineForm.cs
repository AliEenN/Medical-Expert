using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class MedicineAndMedicineForm
    {
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }

        public int MedicineFormId { get; set; }
        public MedicineForm MedicineForm { get; set; }
    }
}
