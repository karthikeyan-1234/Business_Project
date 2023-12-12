using CommonLibrary.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Commands.Purchase_Commands
{
    public class UpdatePurchaseDetailCommand : IRequest<PurchaseDetail>
    {
        public PurchaseDetail updatedPurchaseDetail { get; set; }

        public UpdatePurchaseDetailCommand(PurchaseDetail updatedPurchaseDetail)
        {
            this.updatedPurchaseDetail = updatedPurchaseDetail;
        }
    }
}
