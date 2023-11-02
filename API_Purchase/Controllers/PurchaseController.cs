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

        [HttpPost("AddPurchaseDetails", Name = "AddPurchaseDetails", Order = 6)]
        public async Task<ActionResult> AddPurchaseDetails(NewPurchaseDetailRequest newPurchaseDetailRequest)
        {
            var result = await purchaseService.AddPurchaseDetailAsync(newPurchaseDetailRequest);
            return Ok(result);
        }
    }
}
