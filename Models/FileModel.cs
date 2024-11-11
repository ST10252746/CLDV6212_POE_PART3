namespace ST10252746_CLDV6212_POE_PART3.Models
{
    // FileModel represents file details within the application.
    public class FileModel
    {
        // Name of the file.
        public string Name { get; set; }

        // Size of the file in bytes.
        public long Size { get; set; }

        // Date and time when the file was last modified.
        public DateTimeOffset? LastModified { get; set; }

        // DisplaySize returns a human-readable file size (e.g., in MB, KB).
        public string DisplaySize
        {
            get
            {
                if (Size >= 1024 * 1024)
                    return $"{Size / 1024 / 1024} MB";
                if (Size >= 1024)
                    return $"{Size / 1024} KB";
                return $"{Size} Bytes";
            }
        }
    }
}
