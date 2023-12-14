using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using MediatR;

using Services.CQRS.Commands.Inventory_Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers.Inventory_Handlers
{
    public class ReduceInventoryCommandHandler : IRequestHandler<ReduceInventoryCommand, float>
    {
        IGenericRepository<CommonLibrary.Models.Inventory, InventoryDBContext> _repository;

        public ReduceInventoryCommandHandler(IGenericRepository<CommonLibrary.Models.Inventory, InventoryDBContext> _repository)
        {
            this._repository = _repository;
        }

        public async Task<float> Handle(ReduceInventoryCommand request, CancellationToken cancellationToken)
        {
            var currentInventory = await _repository.GetById(request.item_id);

            if (currentInventory.qty >= request.qty)
            {
                _repository.Update(new CommonLibrary.Models.Inventory() { id = currentInventory.id, itemId = currentInventory.itemId, qty = currentInventory.qty - request.qty, lastUpdated = DateTime.Now, Notes = "Inventory updated" });
                await _repository.SaveChangesAsync();
                return currentInventory.qty - request.qty;
            }

            return default;

        }
    }
}
