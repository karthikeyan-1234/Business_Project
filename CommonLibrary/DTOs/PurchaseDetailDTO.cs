using CommonLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.DTOs
{
    public class PurchaseDetailDTO : BaseModel
    {
        public int id { get; set; }
        public int purchaseId { get; set; }
        public int itemId { get; set; }
        public float qty { get; set; }
        public float rate { get; set; }
        public DateTime lastEditat { get; set; }
    }
}
