using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Models
{
    public class Inventory : BaseModel
    {
        public int id { get; set; }
        public int itemId { get; set; }
        public float qty { get; set; }
        public DateTime lastUpdated { get; set; }
        public string? Notes { get; set; }
    }
}
