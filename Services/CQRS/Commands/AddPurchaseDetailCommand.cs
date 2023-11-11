using CommonLibrary.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Commands
{
    public class AddPurchaseDetailCommand : IRequest<PurchaseDetail>
    {
        public PurchaseDetail newPurchaseDetail { get; set; }

        public AddPurchaseDetailCommand(PurchaseDetail newPurchaseDetail)
        {
            this.newPurchaseDetail = newPurchaseDetail;
        }
    }
}
