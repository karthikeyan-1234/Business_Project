using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Queries.Inventory
{
    public class GetItemInventoryQueryBroker : IRequest<CommonLibrary.Models.Inventory>
    {
        public string item_id { get; set; }

        public GetItemInventoryQueryBroker(string item_id)
        {
            this.item_id = item_id;
        }
    }
}
