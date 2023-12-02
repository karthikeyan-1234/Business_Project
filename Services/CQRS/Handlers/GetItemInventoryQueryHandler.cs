using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using MediatR;

using Services.CQRS.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers
{
    public class GetItemInventoryQueryHandler : IRequestHandler<GetItemInventoryQuery, Inventory>
    {
        IGenericRepository<Inventory, InventoryDBContext> repo;

        public GetItemInventoryQueryHandler(IGenericRepository<Inventory, InventoryDBContext> repo)
        {
            this.repo = repo;
        }

        public async Task<Inventory> Handle(GetItemInventoryQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
