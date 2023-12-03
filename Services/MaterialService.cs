﻿using AutoMapper;

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
    public class MaterialService : IMaterialService
    {
        IGenericRepository<Material, MaterialDBContext> repo;
        IMapper mapper;

        public MaterialService(IGenericRepository<Material, MaterialDBContext> repo,IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task<MaterialDTO> AddMaterialAsync(NewMaterialRequest request)
        {
            var material = mapper.Map<Material>(request);
            var newMaterial = await repo.AddAsync(material);
            await repo.SaveChangesAsync();
            return mapper.Map<MaterialDTO>(newMaterial);
        }

        public async Task DeleteMaterialAsync(MaterialDTO updateMaterial)
        {
            var material = mapper.Map<Material>(updateMaterial);
            repo.Delete(material);
            await repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<MaterialDTO>> GetAllMaterialsAsync()
        {
            return mapper.Map<IEnumerable<MaterialDTO>>(await repo.GetAllAsync());
        }

        public Task<MaterialDTO> GetMaterialDetails(int MaterialId)
        {
           return Task.FromResult(mapper.Map<MaterialDTO>(repo.GetById(MaterialId)));
        }

        public async Task<MaterialDTO> UpdateMaterialAsync(MaterialDTO updateMaterial)
        {
            repo.Update(mapper.Map<Material>(updateMaterial));
            await repo.SaveChangesAsync();
            return updateMaterial;
        }
    }
}