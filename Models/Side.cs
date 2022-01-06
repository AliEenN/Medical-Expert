using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class Side
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string Description { get; set; }

        public ICollection<Category> Categories { get; set; }
        public ICollection<CategorySide> CategoriesSides { get; set; }
    }
}
