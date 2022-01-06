using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        public byte[] Picture { get; set; }

        public ICollection<Side> Sides { get; set; }
        public ICollection<CategorySide> CategoriesSides { get; set; }
        public List<Disease> Diseases { get; set; }
    }
}
