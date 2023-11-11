using CommonLibrary.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Notifications
{
    public class PurchaseDetailAddedNotification : INotification
    {
        public PurchaseDetail newPurchaseDetail { get; set; }

        public PurchaseDetailAddedNotification(PurchaseDetail newPurchaseDetail)
        {
            this.newPurchaseDetail = newPurchaseDetail;
        }
    }
}
