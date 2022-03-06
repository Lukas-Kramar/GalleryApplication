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
    public class DeleteModel : PageModel
    {
        private readonly GalleryApp.Data.ApplicationDbContext _context;

        public DeleteModel(GalleryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Gallery Gallery { get; set; }
        [BindProperty]
        public string UserId { get; set; }
        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

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
            return Page();

        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Gallery = await _context.Galleries.FindAsync(id);

            if (Gallery != null)
            {
                if (Gallery.Name.Contains("Default Gallery")) ErrorMessage = "You can´t delete your Default Gallery!";
                _context.Galleries.Remove(Gallery);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
