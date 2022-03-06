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
using System.Security.Claims;

namespace GalleryApp.Areas.User.CRUD_Gallery
{
    public class EditModel : PageModel
    {
        private readonly GalleryApp.Data.ApplicationDbContext _context;
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public EditModel(GalleryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Gallery Gallery { get; set; }
        [BindProperty]
        public string UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Gallery = await _context.Galleries
                .Include(g => g.Creator).FirstOrDefaultAsync(m => m.Id == id);

            if (Gallery == null)
            {
                return NotFound();
            }
            UserId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value.ToString();
            if (Gallery.CreatorId != UserId) return Unauthorized();
            if (Gallery.Name == "Default Gallery") { ErrorMessage = "You can´t edit Default Gallery!!"; return RedirectToPage("./Index"); }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            _context.Attach(Gallery).State = EntityState.Modified;
            var GalleryPictures = _context.GalleryStoredFiles
                .Include(gs => gs.StoredFile)
                .Where(gs => gs.GalleryId == Gallery.Id)
                .ToList();
            try
            {
                if(Gallery.isPublic && !GalleryPictures.FirstOrDefault().StoredFile.isPublic)
                {
                    foreach(var GalleryPicture in GalleryPictures)
                    {
                        GalleryPicture.StoredFile.isPublic = true;
                        _context.Attach(GalleryPicture.StoredFile).State = EntityState.Modified;
                    }
                }
                if (!Gallery.isPublic && GalleryPictures.FirstOrDefault().StoredFile.isPublic)
                {
                    foreach (var GalleryPicture in GalleryPictures)
                    {
                        GalleryPicture.StoredFile.isPublic = false;
                        _context.Attach(GalleryPicture.StoredFile).State = EntityState.Modified;
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GalleryExists(Gallery.Id))
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

        private bool GalleryExists(int id)
        {
            return _context.Galleries.Any(e => e.Id == id);
        }
    }
}
