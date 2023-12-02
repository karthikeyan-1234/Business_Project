using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.DTOs
{
    public class MaterialDTO
    {
        public int id { get; set; }
        public string ?name { get; set; }
        public string ?description { get; set; }
        public string expiryInDays { get; set; }
    }
}
