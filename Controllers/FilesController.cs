using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST10252746_CLDV6212_POE_PART3.Models;
using ST10252746_CLDV6212_POE_PART3.Services;

namespace ST10252746_CLDV6212_POE_PART3.Controllers
{
    public class FilesController : Controller
    {
        // Declare variable for FileShareService to handle file operations
        private readonly FileShareService _fileShareService;

        // Constructor to inject FileShareService
        public FilesController(FileShareService fileShareService)
        {
            _fileShareService = fileShareService; // Initialize the _fileShareService field
        }

        // Action to display the list of uploaded files
        public async Task<IActionResult> Index()
        {
            List<FileModel> files;

            try
            {
                // Try to fetch the list of files from the "uploads" directory
                files = await _fileShareService.ListFilesAsync("uploads");
            }
            catch (Exception ex)
            {
                // If an error occurs, display the error message and return an empty list
                ViewBag.Message = $"Failed to load files: {ex.Message}";
                files = new List<FileModel>();
            }

            return View(files); // Pass the list of files to the view
        }

        // Action to handle file upload (only accessible by Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Check if no file is selected
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please select a file to upload.");
                return await Index(); // Return to the Index action with an error message
            }

            try
            {
                // Upload the selected file
                using (var stream = file.OpenReadStream())
                {
                    string directoryName = "uploads"; // Define the upload directory
                    string fileName = file.FileName; // Get the file name
                    await _fileShareService.UploadFileAsync(directoryName, fileName, stream); // Upload file to the service
                }

                TempData["Message"] = $"File '{file.FileName}' uploaded successfully!";
            }
            catch (Exception ex)
            {
                // If an error occurs, display the error message
                TempData["Message"] = $"File upload failed: {ex.Message}";
            }

            return RedirectToAction("Index"); // Redirect to the Index action to show the updated file list
        }

        // Action to handle file download
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            // Check if the file name is empty or null
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name cannot be null or empty."); // Return an error response
            }

            try
            {
                // Attempt to fetch the file stream from the service
                var fileStream = await _fileShareService.DownloadFileAsync("uploads", fileName);

                // Check if the file is not found
                if (fileStream == null)
                {
                    return NotFound($"File '{fileName}' not found."); // Return a Not Found response
                }

                return File(fileStream, "application/octet-stream", fileName); // Return the file stream for download
            }
            catch (Exception ex)
            {
                // If an error occurs, display the error message
                return BadRequest($"Error downloading file: {ex.Message}");
            }
        }
    }
}
