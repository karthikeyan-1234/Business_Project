using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Models
{
    public class Material : BaseModel
    {
        public int id { get; set; }
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string? name { get; set; }
        [StringLength(100, ErrorMessage = "Description cannot exceed 100 characters")]
        public string? description { get; set; }
        public int expiryInDays { get; set; }

    }
}
