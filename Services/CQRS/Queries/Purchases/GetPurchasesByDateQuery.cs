using CommonLibrary.DTOs;
using CommonLibrary.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Queries.Purchases
{
    public class GetPurchaseDetailsByDateQuery : IRequest<IEnumerable<PurchaseDetail>>
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public GetPurchaseDetailsByDateQuery(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }
    }
}
