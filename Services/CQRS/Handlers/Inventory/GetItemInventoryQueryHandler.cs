using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using MediatR;

using Services.CQRS.Queries.Inventory_Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers.Inventory_Handlers
{
    public class GetItemInventoryQueryHandler : IRequestHandler<GetItemInventoryQuery, Inventory>
    {
        IGenericRepository<Inventory, InventoryDBContext> repo;

        public GetItemInventoryQueryHandler(IGenericRepository<Inventory, InventoryDBContext> repo)
        {
            this.repo = repo;
        }

        public async Task<Inventory>? Handle(GetItemInventoryQuery request, CancellationToken cancellationToken)
        {
            return repo.Find(i => i.itemId == request.item_id).OrderByDescending(i => i.lastUpdated).FirstOrDefault();
        }
    }
}
