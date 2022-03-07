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

namespace GalleryApp.Areas.User.CRUD_StoredFile
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;

        public DeleteModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
        }

        [BindProperty]
        public StoredFile StoredFile { get; set; }
        [BindProperty]
        public IList<GalleryStoredFile> GalleryStoredFiles { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StoredFile = await _context.Files
                .Include(s => s.Uploader).FirstOrDefaultAsync(m => m.Id == id);
            
            if (StoredFile == null)
            {
                return NotFound();
            }
            GalleryStoredFiles = _context.GalleryStoredFiles.Include(s => s.Gallery).Where(g => g.StoredFileId == id).ToList();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            StoredFile = await _context.Files.FindAsync(id);
            if (StoredFile != null)
            {
                _context.Files.Remove(StoredFile);
                //await _context.SaveChangesAsync();
                GalleryStoredFiles = _context.GalleryStoredFiles.Include(s => s.Gallery).Where(g => g.StoredFileId == id).ToList();
                foreach(var g in GalleryStoredFiles)
                {
                    if (g.Gallery.IdThumbnail == id) g.Gallery.IdThumbnail = null; //pokud byl obrázek thumbnailem v nějaké galerii
                }
                //Vymaže z fyzického disku
                
                var path = Path.Combine("Uploads", id.ToString());
                System.IO.File.Delete(path);
                if (GalleryStoredFiles.Count() > 0)
                {
                    foreach (GalleryStoredFile g in GalleryStoredFiles)
                    {
                        _context.GalleryStoredFiles.Remove(g);
                    }
                }
                await _context.SaveChangesAsync();
            }
            SuccessMessage = "Picture Deleted";
            return RedirectToPage("../CRUD_Gallery/Index");

        }
    }
}
