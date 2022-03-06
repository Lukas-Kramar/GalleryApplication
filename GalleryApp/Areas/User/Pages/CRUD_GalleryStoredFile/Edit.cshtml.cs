#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GalleryApp.Data;
using GalleryApp.Models;

namespace GalleryApp.Areas.User.CRUD_GalleryStoredFile
{
    public class EditModel : PageModel
    {
        private readonly GalleryApp.Data.ApplicationDbContext _context;

        public EditModel(GalleryApp.Data.ApplicationDbContext context)
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
           ViewData["GalleryId"] = new SelectList(_context.Galleries, "Id", "CreatorId");
           ViewData["StoredFileId"] = new SelectList(_context.Files, "Id", "ContentType");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(GalleryStoredFile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GalleryStoredFileExists(GalleryStoredFile.StoredFileId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool GalleryStoredFileExists(Guid id)
        {
            return _context.GalleryStoredFiles.Any(e => e.StoredFileId == id);
        }
    }
}
