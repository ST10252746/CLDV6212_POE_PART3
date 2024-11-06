namespace ST10252746_CLDV6212_POE_PART3.Models
{
    public class OrderAdminViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserEmail { get; set; }
        public string? Status { get; set; }

        public decimal TotalPrice { get; set; }
    }
}