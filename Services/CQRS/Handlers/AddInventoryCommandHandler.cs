using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;
using MediatR;
using Services.CQRS.Commands;


namespace Services.CQRS.Handlers
{
    public class AddInventoryCommandHandler : IRequestHandler<AddInventoryCommand, Inventory>
    {
        IGenericRepository<Inventory, InventoryDBContext> _repo;

        public AddInventoryCommandHandler(IGenericRepository<Inventory, InventoryDBContext> _repo)
        {
            this._repo = _repo;
        }

        public async Task<Inventory> Handle(AddInventoryCommand request, CancellationToken cancellationToken)
        {
            var inventory = await _repo.AddAsync(request.newInventory);
            await _repo.SaveChangesAsync();
            return inventory;
        }
    }
}
