using GalleryApp.Data;
using GalleryApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;

namespace GalleryApp.Pages
{
    public class IndexModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }    
        public List<StoredFileListViewModel> StoredFiles { get; set; }
        //public List<StoredFile>

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment environment, ApplicationDbContext context)
        {
            _environment = environment;
            _logger = logger;
            _context = context;
        }
        public void OnGet()
        {
            StoredFiles = _context.Files
              .Where(f => f.isPublic == true)
              .AsNoTracking()
              .Include(f => f.Uploader)
              .Include(f => f.Thumbnails)
              .Select(f => new StoredFileListViewModel
              {
                  Id = f.Id,
                  ContentType = f.ContentType,
                  OriginalName = f.OriginalName,
                  UploaderId = f.UploaderId,
                  Uploader = f.Uploader,
                  UploadedAt = f.UploadedAt,
                  ThumbnailCount = f.Thumbnails.Count
              })              
              .OrderBy(f => f.UploadedAt)
              .Take(12)
              .ToList();

            //var fullNames = Directory.GetFiles(Path.Combine(_environment.ContentRootPath, "Uploads")).ToList();
            //    foreach (var fn in fullNames)
            //    {
            //        Files.Add(Path.GetFileName(fn));
            //    }           
        }
        public IActionResult OnGetDownload(string filename)
        {
            var fullName = Path.Combine(_environment.ContentRootPath, "Uploads", filename);
            if (System.IO.File.Exists(fullName)) // existuje soubor na disku?
            {
                var fileRecord = _context.Files.Find(Guid.Parse(filename));
                if (fileRecord != null) // je soubor v databázi?
                {
                    return PhysicalFile(fullName, fileRecord.ContentType, fileRecord.OriginalName);
                    // vrať ho zpátky pod původním názvem a typem
                }
                else
                {
                    ErrorMessage = "There is no record of such file.";
                    return RedirectToPage();
                }
            }
            else
            {
                ErrorMessage = "There is no such file.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnGetThumbnail(string filename, ThumbnailType type = ThumbnailType.Square)
        {
            StoredFile file = await _context.Files
              .AsNoTracking()
              .Where(f => f.Id == Guid.Parse(filename))
              .SingleOrDefaultAsync();
            if (file == null)
            {
                return NotFound("no record for this file");
            }
            Thumbnail thumbnail = await _context.Thumbnails
              .AsNoTracking()
              .Where(t => t.FileId == Guid.Parse(filename) && t.Type == type)
              .SingleOrDefaultAsync();
            if (thumbnail != null)
            {
                return File(thumbnail.Blob, file.ContentType);
            }
            return NotFound("no thumbnail for this file");
        }
    }
}