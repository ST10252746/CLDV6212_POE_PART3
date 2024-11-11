using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace ST10252746_CLDV6212_POE_PART3.Models
{
    // Product represents an item available for order within the application.
    public class Product
    {
        // Unique identifier for the product.
        public int ProductId { get; set; }

        // Name of the product.
        public string? Name { get; set; }

        // Description of the product.
        public string? ProductDescription { get; set; }

        // Price of the product, formatted as currency.
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal? Price { get; set; }

        // Category of the product (e.g., electronics, clothing).
        public string? Category { get; set; }

        // Availability status of the product (true if available).
        public bool? Availability { get; set; }

        // URL path to the product's image.
        public string? ImageUrlpath { get; set; }
    }
}
