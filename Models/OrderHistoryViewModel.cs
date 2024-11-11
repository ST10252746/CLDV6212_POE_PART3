namespace ST10252746_CLDV6212_POE_PART3.Models
{
    // OrderHistoryViewModel is a view model for displaying order history to users.
    public class OrderHistoryViewModel
    {
        // Unique identifier for the order.
        public int OrderId { get; set; }

        // Date when the order was created.
        public DateTime OrderDate { get; set; }

        // Current status of the order.
        public string Status { get; set; }

        // Total price of the order.
        public decimal TotalPrice { get; set; }
    }
}
