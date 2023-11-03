using CommonLibrary.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API_Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        IInventoryService inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            this.inventoryService = inventoryService;
        }

        [HttpPost("AddNewInventoryAsync", Name = "AddNewInventoryAsync", Order = 1)]
        public async Task<ActionResult> AddNewInventoryAsync(NewInventoryRequest request)
        {
            var newInventory = await inventoryService.AddInventoryAsync(request);
            return Ok(newInventory);
        }
    }
}
