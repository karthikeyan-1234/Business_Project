using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Commands.Inventory_Commands
{
    public class ReduceInventoryCommand : IRequest<float>
    {
        public int item_id { get; set; }
        public float qty { get; set; }
    }
}
