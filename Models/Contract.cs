using System.ComponentModel.DataAnnotations;

namespace ST10252746_CLDV6212_POE_PART3.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Description field is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The FilePath field is required.")]
        public string FilePath { get; set; } // Changed from FileName to FilePath

        public DateTime UploadDate { get; set; }
    }
}
