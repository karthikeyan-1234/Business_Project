using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Models.Requests
{
    public class NewPurchaseDetailRequest
    {
        public int purchaseId { get; set; }
        public int itemId { get; set; }
        public float qty { get; set; }
        public float rate { get; set; }
        public DateTime lastEditat { get; set; }
    }
}
