using MedicalExpert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.ViewModels
{
    public class DiagnosisViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
    }

    public class DiagnosisDefineSidesViewModel
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<CheckBoxViewModel> SidesCheckBoxes { get; set; }
    }

    public class DiseasAndMedicineViewModel
    {
        public List<Disease> Diseases { get; set; }
        public List<Medicine> Medicines { get; set; }
        public string NotFoundMedicines { get; set; }
    }
}
