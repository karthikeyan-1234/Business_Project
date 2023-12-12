using CommonLibrary.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Commands.Purchase_Commands
{
    public class DeletePurchaseDetailCommand : IRequest<PurchaseDetail>
    {
        public PurchaseDetail deletePurchaseDetail;

        public DeletePurchaseDetailCommand(PurchaseDetail deletePurchaseDetail)
        {
            this.deletePurchaseDetail = deletePurchaseDetail;
        }
    }
}
