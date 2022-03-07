#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GalleryApp.Data;
using GalleryApp.Models;
using System.Security.Claims;

namespace GalleryApp.Areas.User.CRUD_Gallery
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;
        private readonly ILogger<IndexModel> _logger;
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }


        public DetailsModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Gallery Gallery { get; set; }
        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public List<StoredFileListViewModel> StoredFilesView { get; set; }
        public string Owner { get; set; }
        public string HeaderMessage { get; set; }
        public GalleryStoredFile GalleryStoredFile { get; set; }
        public List<GalleryStoredFile> GalleryStoredFiles { get; set; }
        public StoredFile StoredFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Gallery = await _context.Galleries                
                .Include(g => g.Creator)
                .Include(g => g.StoredPictures)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Gallery == null)
            {
                return NotFound();
            }
            UserId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value.ToString();
            if (Gallery.CreatorId == UserId || Gallery.isPublic == true)
            {

                Owner = (Gallery.Creator.NickName == null) ? Gallery.Creator.Email : Gallery.Creator.NickName;
                HeaderMessage = $"{Owner}´s Gallery named {Gallery.Name}.";
                StoredFilesView = _context.GalleryStoredFiles
                    .Include(gs => gs.StoredFile)
                    .Include(gs => gs.StoredFile.Uploader)
                    .Include(gs => gs.StoredFile.Thumbnails)
                    .Where(gs => gs.GalleryId == Gallery.Id)
                    .Select(f => new StoredFileListViewModel
                    {
                        Id = f.StoredFile.Id,
                        ContentType = f.StoredFile.ContentType,
                        OriginalName = f.StoredFile.OriginalName,
                        UploaderId = f.StoredFile.UploaderId,
                        Uploader = f.StoredFile.Uploader,
                        UploadedAt = f.StoredFile.UploadedAt,
                        ThumbnailCount = f.StoredFile.Thumbnails.Count
                    })
                    .ToList();
                return Page();
            }
            return Unauthorized();
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

        //public async Task<IActionResult> OnPostDeleteFromGallery (Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    GalleryStoredFile = await _context.GalleryStoredFiles.FindAsync(id);

        //    if (GalleryStoredFile != null)
        //    {
        //        _context.GalleryStoredFiles.Remove(GalleryStoredFile);
        //        await _context.SaveChangesAsync();
        //        SuccessMessage = "Deleted from Gallery";
        //    }

        //    return RedirectToPage("./Index");
            
        //}

        public async Task<IActionResult> OnGetMakethumbnail(string storedfileid, string galleryid)
        {
           
            if (storedfileid == null)
            {
                return NotFound();
            }

            StoredFile file = await _context.Files
              .AsNoTracking()
              .Where(f => f.Id == Guid.Parse(storedfileid))
              .SingleOrDefaultAsync();
            
            if (file == null)
            {
                return NotFound();
            }

            Gallery = await _context.Galleries    
              .Include(g => g.StoredPictures)
              .Where(g => g.Id == Int32.Parse(galleryid))
              .SingleOrDefaultAsync();            
            var galleryStoredFile = _context.GalleryStoredFiles.Where(g => g.GalleryId == Gallery.Id).Where(g => g.StoredFileId == file.Id).FirstOrDefault();            
            if ( galleryStoredFile != null)
            {
                Gallery.IdThumbnail = file.Id;
                _context.Attach(Gallery).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                SuccessMessage = "Changed gallery thumbnail";
            }
            else
            {
                ErrorMessage = "Picture isnt´t in this gallery";
            }
            return RedirectToPage("./Index");

        }

    }
}