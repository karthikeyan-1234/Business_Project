using CommonLibrary.DTOs;
using CommonLibrary.Models.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API_Material.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        IPurchaseService purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            this.purchaseService = purchaseService;
        }

        [HttpPost("AddNewPurchaseAsync", Name = "AddNewPurchaseAsync", Order = 1)]
        public async Task<ActionResult> AddNewPurchaseAsync(NewPurchaseRequest request)
        {
                var newPurchase = await purchaseService.AddPurchaseAsync(request);
                return Ok(newPurchase);
        }

        [HttpGet("GetAllPurchasesAsync", Name = "GetAllPurchasesAsync", Order = 2)]
        public async Task<ActionResult> GetAllPurchasesAsync()
        {
            var purchases = await purchaseService.GetAllPurchasesAsync();
            return Ok(purchases);
        }

        [HttpPut("UpdatePurchaseAsync", Name = "UpdatePurchaseAsync", Order = 3)]
        public async Task<ActionResult> UpdatePurchaseAsync(PurchaseDTO updatePurchase)
        {
            var purchase = await purchaseService.UpdatePurchaseAsync(updatePurchase);
            return Ok(purchase);
        }

        [HttpPut("DeletePurchaseAsync", Name = "DeletePurchaseAsync", Order = 4)]
        public async Task<ActionResult> DeletePurchaseAsync(PurchaseDTO deletePurchase)
        {
            await purchaseService.DeletePurchaseAsync(deletePurchase);
            return Ok();
        }

        [HttpGet("GetPurchaseDetails", Name = "GetPurchaseDetails", Order = 5)]
        public async Task<ActionResult> GetPurchaseDetails(int purchaseId)
        {
            var result = await purchaseService.GetPurchaseDetails(purchaseId);
            return Ok(result);
        }

        [HttpGet("GetInventoryForItem", Name = "GetInventoryForItem", Order = 6)]
        public async Task<ActionResult> GetInventoryForItem(int item_id)
        {
            var result = await purchaseService.GetInventoryStatus(item_id);
            return Ok(result);
        }

        [HttpPost("AddPurchaseDetails", Name = "AddPurchaseDetails", Order = 7)]
        public async Task<ActionResult> AddPurchaseDetails(NewPurchaseDetailRequest newPurchaseDetailRequest)
        {
            var result = await purchaseService.AddPurchaseDetailAsync(newPurchaseDetailRequest);
            return Ok(result);
        }

        [HttpPut("UpdatePurchaseDetails", Name = "UpdatePurchaseDetails", Order = 8)]
        public async Task<ActionResult> UpdatePurchaseDetails(UpdatePurchaseDetailRequest updatedPurchaseDetailRequest)
        {
            var result = await purchaseService.UpdatePurchaseDetailAsync(updatedPurchaseDetailRequest);
            return Ok(result);
        }

        [HttpDelete("DeletePurchaseDetails", Name = "DeletePurchaseDetails", Order = 9)]
        public async Task<ActionResult> DeletePurchaseDetails(UpdatePurchaseDetailRequest deletePurchaseDetailRequest)
        {
            var result = await purchaseService.DeletePurchaseDetailAsync(deletePurchaseDetailRequest);
            return Ok(result);
        }
    }
}
