using CommonLibrary.DTOs;
using CommonLibrary.Models.Requests;

namespace Services
{
    public interface IPurchaseService
    {
        Task<PurchaseDTO> AddPurchaseAsync(NewPurchaseRequest request);
        Task<IEnumerable<PurchaseResultDTO>> GetAllPurchasesAsync();
        Task<PurchaseDTO> UpdatePurchaseAsync(PurchaseDTO updatePurchase);
        Task DeletePurchaseAsync(PurchaseDTO updatePurchase);
        Task<IEnumerable<PurchaseDetailDTO>> GetPurchaseDetails(int purchaseId);
        Task<PurchaseDetailDTO> AddPurchaseDetailAsync(NewPurchaseDetailRequest request);

    }
}