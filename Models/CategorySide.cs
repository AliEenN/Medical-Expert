using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class CategorySide
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int SideId { get; set; }
        public Side Side { get; set; }
    }
}
