using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GiveNTake.Model
{
    public class NewProductDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
    }
}
