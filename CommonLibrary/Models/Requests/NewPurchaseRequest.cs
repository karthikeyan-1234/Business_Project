using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Models.Requests
{
    public class NewPurchaseRequest
    {
        public DateTime? purchaseDate { get; set; }
        public int vendorId { get; set; }
    }
}
