using CommonLibrary.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Queries
{
    public class GetItemInventoryQuery : IRequest<Inventory>
    {
        public int item_id { get; set; }

        public GetItemInventoryQuery(int item_id)
        {
            this.item_id = item_id;
        }
    }
}
