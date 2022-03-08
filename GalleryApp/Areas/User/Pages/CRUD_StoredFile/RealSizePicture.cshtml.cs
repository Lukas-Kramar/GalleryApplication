using GalleryApp.Areas.User.CRUD_Gallery;
using GalleryApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GalleryApp.Areas.User.Pages.CRUD_StoredFile
{
    public class RealSizePictureModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;
        private readonly ILogger<IndexModel> _logger;        
        public IActionResult OnGetFullSize(string filename)
        {
            var fullName = Path.Combine(_environment.ContentRootPath, "Uploads", filename);

            if (System.IO.File.Exists(fullName))
            {
                var fileRecord = _context.Files.Find(Guid.Parse(filename));

                if (fileRecord != null)
                {
                    return  File(System.IO.File.OpenRead(fullName), fileRecord.ContentType);
                }
                else
                {
                    return RedirectToPage();
                }
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
