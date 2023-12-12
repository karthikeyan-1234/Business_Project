using CommonLibrary.DTOs;
using CommonLibrary.Models.Requests;

namespace Services
{
    public interface IInventoryService
    {
        Task<InventoryDTO> AddInventoryAsync(NewInventoryRequest request);
        Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync();
        Task<InventoryDTO> UpsertInventoryAsync(InventoryDTO updateInventory);
        Task DeleteInventoryAsync(InventoryDTO updateInventory);
        Task<InventoryDTO> GetItemInventory(int item_id);
    }
}