using CommonLibrary.DTOs;
using CommonLibrary.Models.Requests;

namespace Services
{
    public interface IInventoryService
    {
        Task<InventoryDTO> AddInventoryAsync(NewInventoryRequest newInventory);
    }
}