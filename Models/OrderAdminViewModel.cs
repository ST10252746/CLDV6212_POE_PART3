namespace ST10252746_CLDV6212_POE_PART3.Models
{
    // OrderAdminViewModel is a view model for displaying order details in an admin view.
    public class OrderAdminViewModel
    {
        // Unique identifier for the order.
        public int OrderId { get; set; }

        // Date when the order was created.
        public DateTime OrderDate { get; set; }

        // Email of the user who placed the order.
        public string UserEmail { get; set; }

        // Status of the order.
        public string? Status { get; set; }

        // Total price of the order.
        public decimal TotalPrice { get; set; }
    }
}
