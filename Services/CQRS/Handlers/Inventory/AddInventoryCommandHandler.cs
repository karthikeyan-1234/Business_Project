using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using MediatR;

using Services.CQRS.Commands.Inventory_Commands;

namespace Services.CQRS.Handlers.Inventory_Handlers
{
    public class AddInventoryCommandHandler : IRequestHandler<AddInventoryCommand, CommonLibrary.Models.Inventory>
    {
        IGenericRepository<CommonLibrary.Models.Inventory, InventoryDBContext> _repo;

        public AddInventoryCommandHandler(IGenericRepository<CommonLibrary.Models.Inventory, InventoryDBContext> _repo)
        {
            this._repo = _repo;
        }

        public async Task<CommonLibrary.Models.Inventory> Handle(AddInventoryCommand request, CancellationToken cancellationToken)
        {
            var inventory = await _repo.AddAsync(request.newInventory);
            await _repo.SaveChangesAsync();
            return inventory;
        }
    }
}
