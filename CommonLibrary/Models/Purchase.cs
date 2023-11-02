
namespace CommonLibrary.Models
{
    public class Purchase : BaseModel
    {
        public int id { get; set; }
        public DateTime? purchaseDate { get; set; }
        public int vendorId { get; set; }

        public ICollection<PurchaseDetail> ?PurchaseDetails { get; set; }
    }
}