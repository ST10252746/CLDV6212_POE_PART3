using System.ComponentModel.DataAnnotations.Schema;

namespace ST10252746_CLDV6212_POE_PART3.Models
{
    // Order represents an order made by a user.
    public class Order
    {
        // Unique identifier for the order.
        public int OrderId { get; set; }

        // ID of the user who placed the order.
        public string UserId { get; set; }

        // Date when the order was created.
        public DateTime OrderDate { get; set; }

        // Status of the order (e.g., pending, shipped, delivered).
        public string? Status { get; set; }

        // Total price of the order.
        public decimal? TotalPrice { get; set; }

        // Collection of order requests associated with this order.
        public virtual ICollection<OrderRequest> OrderRequests { get; set; } = new List<OrderRequest>();

        // Reference to the ApplicationUser who placed the order, with UserId as the foreign key.
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
