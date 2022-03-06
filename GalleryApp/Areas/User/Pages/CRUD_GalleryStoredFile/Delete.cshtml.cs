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

namespace GalleryApp.Areas.User.CRUD_GalleryStoredFile
{
    public class DeleteModel : PageModel
    {
        private readonly GalleryApp.Data.ApplicationDbContext _context;

        public DeleteModel(GalleryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public GalleryStoredFile GalleryStoredFile { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GalleryStoredFile = await _context.GalleryStoredFiles
                .Include(g => g.Gallery)
                .Include(g => g.StoredFile).FirstOrDefaultAsync(m => m.StoredFileId == id);

            if (GalleryStoredFile == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            GalleryStoredFile = await _context.GalleryStoredFiles.FindAsync(id);

            if (GalleryStoredFile != null)
            {
                _context.GalleryStoredFiles.Remove(GalleryStoredFile);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
