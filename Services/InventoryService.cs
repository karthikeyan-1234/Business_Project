using AutoMapper;
using CommonLibrary.Caching;
using CommonLibrary.Contexts;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using CommonLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class InventoryService : IInventoryService
    {
        IGenericRepository<Inventory, InventoryDBContext> repo;
        IMapper mapper;
        ICacheManager cache;

        public InventoryService(IGenericRepository<Inventory, InventoryDBContext> repo, IMapper mapper, ICacheManager cache)
        {
            this.mapper = mapper;
            this.cache = cache;
            this.repo = repo;
        }

        public async Task<InventoryDTO> AddInventoryAsync(NewInventoryRequest newInventory)
        {
            var inventory = mapper.Map<Inventory>(newInventory);
            var newInvent = await repo.AddAsync(inventory);
            await repo.SaveChangesAsync();

            return mapper.Map<InventoryDTO>(newInvent);
        }
    }
}
