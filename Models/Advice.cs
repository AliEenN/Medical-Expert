using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class Advice
    {
        public int Id { get; set; }

        [Required, MaxLength(75)]
        public string Title { get; set; }

        [Required, MaxLength(250)]
        public string Description { get; set; }
        public byte[] Picture { get; set; }
    }
}
