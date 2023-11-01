using AutoMapper;
using CommonLibrary.Contexts;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using CommonLibrary.Repositories;

namespace Services
{
    public class PurchaseService : IPurchaseService
    {
        IGenericRepository<Purchase, PurchaseDBContext> purchaseRepo;
        IMapper mapper;

        public PurchaseService(IGenericRepository<Purchase, PurchaseDBContext> purchaseRepo, IMapper mapper)
        {
            this.purchaseRepo = purchaseRepo;
            this.mapper = mapper;
        }

        public async Task<PurchaseDTO> AddPurchaseAsync(NewPurchaseRequest request)
        {
            var newPurchase = new Purchase();
            newPurchase = mapper.Map<Purchase>(request);
            var addedPurchase = await purchaseRepo.AddAsync(newPurchase);
            await purchaseRepo.SaveChangesAsync();
            return mapper.Map<PurchaseDTO>(addedPurchase);
        }

        public async Task DeletePurchaseAsync(PurchaseDTO updatePurchase)
        {
            var purchase = mapper.Map<Purchase>(updatePurchase);
            purchaseRepo.Delete(purchase);
            await purchaseRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<PurchaseDTO>> GetAllPurchasesAsync()
        {
            var purchases = await purchaseRepo.GetAllAsync();
            return mapper.Map<IEnumerable<PurchaseDTO>>(purchases);
        }

        public async Task<PurchaseDTO> UpdatePurchaseAsync(PurchaseDTO updatePurchase)
        {
            var purchase = purchaseRepo.Update(mapper.Map<Purchase>(updatePurchase));
            await purchaseRepo.SaveChangesAsync();
            return mapper.Map<PurchaseDTO>(purchase);

        }
    }
}