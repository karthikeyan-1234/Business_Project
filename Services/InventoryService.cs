using AutoMapper;
using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using CommonLibrary.Repositories;
using MediatR;
using Services.CQRS.Commands;
using Services.CQRS.Commands.Inventory_Commands;
using Services.CQRS.Queries;
using Services.CQRS.Queries.Inventory_Queries;

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

        public async Task<InventoryDTO> GetItemInventory(int item_id)
        {
            var result = await mediator.Send(new GetItemInventoryQuery(item_id));
            return mapper.Map<InventoryDTO>(result);
        }

        public async Task<InventoryDTO> UpsertInventoryAsync(InventoryDTO updateInventory)
        {
            InventoryDTO? returnObject = null;
            var result = await GetItemInventory(updateInventory.itemId);

            if ( result != null)
            {
                updateInventory.id = result.id;
                var Inventory = InventoryRepo.Update(mapper.Map<Inventory>(updateInventory));
                returnObject =  mapper.Map<InventoryDTO>(Inventory);
            }
            else
            {
                var Inventory = await InventoryRepo.AddAsync(mapper.Map<Inventory>(updateInventory));
                returnObject =  mapper.Map<InventoryDTO>(Inventory);
            }

            await InventoryRepo.SaveChangesAsync();
            return returnObject;
        }
    }
}
