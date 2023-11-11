using AutoMapper;
using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using CommonLibrary.Repositories;
using MediatR;
using Services.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class InventoryService : IInventoryService
    {
        IGenericRepository<Inventory, InventoryDBContext> InventoryRepo;
        IMapper mapper;
        ICacheManager cache;
        IMediator mediator;

        public InventoryService(IGenericRepository<Inventory, InventoryDBContext> InventoryRepo, IMapper mapper, ICacheManager cache,IMediator mediator)
        {
            this.mapper = mapper;
            this.cache = cache;
            this.InventoryRepo = InventoryRepo;
            this.mediator = mediator;
        }

        public async Task<InventoryDTO> AddInventoryAsync(NewInventoryRequest newInventory)
        {
            var inventory = mapper.Map<Inventory>(newInventory);
            var result = await mediator.Send(new AddInventoryCommand(inventory));
            return mapper.Map<InventoryDTO>(result);
        }

        public async Task DeleteInventoryAsync(InventoryDTO updateInventory)
        {
            var Inventory = mapper.Map<Inventory>(updateInventory);
            InventoryRepo.Delete(Inventory);
            await InventoryRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryDTO>> GetAllInventoriesAsync()
        {
            var Inventories = await cache.TryGetAsync<IEnumerable<Inventory>>("GetAllInventories");
            if (Inventories is null)
            {
                Inventories = await InventoryRepo.GetAllAsync();
                await cache.TrySetAsync(Inventories, "GetAllInventories");
            }
            return mapper.Map<IEnumerable<InventoryDTO>>(Inventories);
        }

        public async Task<InventoryDTO> UpdateInventoryAsync(InventoryDTO updateInventory)
        {
            var Inventory = InventoryRepo.Update(mapper.Map<Inventory>(updateInventory));
            await InventoryRepo.SaveChangesAsync();
            return mapper.Map<InventoryDTO>(Inventory);

        }
    }
}
