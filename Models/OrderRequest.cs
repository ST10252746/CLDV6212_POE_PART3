namespace ST10252746_CLDV6212_POE_PART3.Models
{
    // OrderRequest represents individual items or requests within an order.
    public class OrderRequest
    {
        // Unique identifier for the order request.
        public int OrderRequestId { get; set; }

        // ID of the associated order.
        public int OrderId { get; set; }

        // ID of the product in the order request.
        public int ProductId { get; set; }

        // Status of the order request (e.g., pending, fulfilled).
        public string? OrderStatus { get; set; }

        // Reference to the associated order.
        public virtual Order Order { get; set; } = null!;

        // Reference to the associated product.
        public virtual Product Product { get; set; } = null!;
    }
}
