using CommonLibrary.Models;

using MediatR;


namespace Services.CQRS.Commands.Inventory_Commands
{
    public class AddInventoryCommand : IRequest<Inventory>
    {
        public Inventory newInventory { get; set; }

        public AddInventoryCommand(Inventory newInventory)
        {
            this.newInventory = newInventory;
        }
    }
}
