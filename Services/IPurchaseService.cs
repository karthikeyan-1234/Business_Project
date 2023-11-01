using CommonLibrary.DTOs;
using CommonLibrary.Models.Requests;

namespace Services
{
    public interface IPurchaseService
    {
        Task<PurchaseDTO> AddPurchaseAsync(NewPurchaseRequest request);
        Task<IEnumerable<PurchaseDTO>> GetAllPurchasesAsync();
        Task<PurchaseDTO> UpdatePurchaseAsync(PurchaseDTO updatePurchase);
        Task DeletePurchaseAsync(PurchaseDTO updatePurchase);
    }
}