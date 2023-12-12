using CommonLibrary.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Notifications.Purchase_Notifications
{
    public class PurchaseAddedNotification : INotification
    {
        public Purchase newPurchase { get; set; }

        public PurchaseAddedNotification(Purchase newPurchase)
        {
            this.newPurchase = newPurchase;
        }
    }
}
