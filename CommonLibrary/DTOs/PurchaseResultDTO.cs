using CommonLibrary.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.DTOs
{
    public class PurchaseResultDTO : BaseModel
    {
        public int id { get; set; }
        public DateOnly purchaseDate { get; set; }
        public int vendorId { get; set; }
    }
}
