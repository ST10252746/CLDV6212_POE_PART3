namespace ST10252746_CLDV6212_POE_PART3.Models
{
    public class OrderRequest
    {
        public int OrderRequestId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string? OrderStatus { get; set; }

        public virtual Order Order { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}