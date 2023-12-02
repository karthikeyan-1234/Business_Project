using CommonLibrary.DTOs;
using CommonLibrary.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API_Material.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        IMaterialService MaterialService;

        public MaterialController(IMaterialService MaterialService)
        {
            this.MaterialService = MaterialService;
        }

        [HttpPost("AddNewMaterialAsync", Name = "AddNewMaterialAsync", Order = 1)]
        public async Task<ActionResult> AddNewMaterialAsync(NewMaterialRequest request)
        {
                var newMaterial = await MaterialService.AddMaterialAsync(request);
                return Ok(newMaterial);
        }

        [HttpGet("GetAllMaterialsAsync", Name = "GetAllMaterialsAsync", Order = 2)]
        public async Task<ActionResult> GetAllMaterialsAsync()
        {
            var Materials = await MaterialService.GetAllMaterialsAsync();
            return Ok(Materials);
        }

        [HttpPut("UpdateMaterialAsync", Name = "UpdateMaterialAsync", Order = 3)]
        public async Task<ActionResult> UpdateMaterialAsync(MaterialDTO updateMaterial)
        {
            var Material = await MaterialService.UpdateMaterialAsync(updateMaterial);
            return Ok(Material);
        }

        [HttpPut("DeleteMaterialAsync", Name = "DeleteMaterialAsync", Order = 4)]
        public async Task<ActionResult> DeleteMaterialAsync(MaterialDTO deleteMaterial)
        {
            await MaterialService.DeleteMaterialAsync(deleteMaterial);
            return Ok();
        }
    }
}
