using CommonLibrary.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Notifications.Inventory_Notifications
{
    public class UpdateInventoryNotification : INotification
    {
        public PurchaseDetail revisedPurchaseDetail { get; set; }
        public Guid guid { get; set; }

        public UpdateInventoryNotification(PurchaseDetail revisedPurchaseDetail)
        {
            this.revisedPurchaseDetail = revisedPurchaseDetail;
        }
    }
}
