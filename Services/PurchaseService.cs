using AutoMapper;
using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using CommonLibrary.Repositories;
using MediatR;
using Services.CQRS.Commands;
using Services.CQRS.Notifications;
using System.Text.Json;

namespace Services
{
    public class PurchaseService : IPurchaseService
    {
        IGenericRepository<Purchase, PurchaseDBContext> purchaseRepo;
        IGenericRepository<PurchaseDetail,PurchaseDBContext> purchaseDetailRepo;
        IMapper mapper;
        ICacheManager cache;
        IMediator mediator;

        public PurchaseService(IGenericRepository<Purchase, PurchaseDBContext> purchaseRepo, IGenericRepository<PurchaseDetail, PurchaseDBContext> purchaseDetailRepo, 
            IMapper mapper,ICacheManager cache,IMediator mediator)
        {
            this.purchaseRepo = purchaseRepo;
            this.purchaseDetailRepo = purchaseDetailRepo;
            this.mapper = mapper;
            this.cache = cache;
            this.mediator = mediator;
        }

        public async Task<PurchaseDTO> AddPurchaseAsync(NewPurchaseRequest request)
        {
            var newPurchase = mapper.Map<Purchase>(request);
            var result = await mediator.Send(new AddPurchaseCommand(newPurchase));
            var msg = new PurchaseAddedNotification(result);
            await mediator.Publish(msg);
            return mapper.Map<PurchaseDTO>(result);
        }

        public async Task DeletePurchaseAsync(PurchaseDTO updatePurchase)
        {
            var purchase = mapper.Map<Purchase>(updatePurchase);
            purchaseRepo.Delete(purchase);
            await purchaseRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<PurchaseResultDTO>> GetAllPurchasesAsync()
        {
            var purchases = await cache.TryGetAsync<IEnumerable<Purchase>>("GetAllPurchases");
            if (purchases is null)
            {
                purchases = await purchaseRepo.GetAllAsync();
                await cache.TrySetAsync(purchases, "GetAllPurchases");
            }
            return mapper.Map<IEnumerable<PurchaseResultDTO>>(purchases);
        }

        public async Task<PurchaseDTO> UpdatePurchaseAsync(PurchaseDTO updatePurchase)
        {
            var purchase = purchaseRepo.Update(mapper.Map<Purchase>(updatePurchase));
            await purchaseRepo.SaveChangesAsync();
            return mapper.Map<PurchaseDTO>(purchase);

        }

        private async Task<Purchase> GetPurchase(int id)
        {
            return await purchaseRepo.GetById(id);
        }

        public async Task<IEnumerable<PurchaseDetailDTO>> GetPurchaseDetails(int purchaseId)
        {
            var purchase = await purchaseRepo.GetById(purchaseId);
            return mapper.Map<IEnumerable<PurchaseDetailDTO>>(purchase.PurchaseDetails);
        }

        public async Task<PurchaseDetailDTO> AddPurchaseDetailAsync(NewPurchaseDetailRequest request)
        {
            var newPurchaseDetail = mapper.Map<PurchaseDetail>(request);

            var result = await mediator.Send(new AddPurchaseDetailCommand(newPurchaseDetail));
            var msg = new PurchaseDetailAddedNotification(result);
            await mediator.Publish(msg);
            return mapper.Map<PurchaseDetailDTO>(result);
        }
    }
}