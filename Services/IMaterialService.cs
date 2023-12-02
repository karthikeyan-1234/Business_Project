using CommonLibrary.DTOs;
using CommonLibrary.Models.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMaterialService
    {
        Task<MaterialDTO> AddMaterialAsync(NewMaterialRequest request);
        Task<IEnumerable<MaterialDTO>> GetAllMaterialsAsync();
        Task<MaterialDTO> UpdateMaterialAsync(MaterialDTO updateMaterial);
        Task DeleteMaterialAsync(MaterialDTO updateMaterial);
        Task<MaterialDTO> GetMaterialDetails(int MaterialId);
    }
}
