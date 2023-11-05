using CommonLibrary.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Commands
{
    public class AddPurchaseCommand : IRequest<Purchase>
    {
        public Purchase newPurchase { get; set; }

        public AddPurchaseCommand(Purchase newPurchase)
        {
            this.newPurchase = newPurchase;
        }
    }
}
