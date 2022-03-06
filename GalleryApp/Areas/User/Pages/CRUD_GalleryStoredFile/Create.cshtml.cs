#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GalleryApp.Data;
using GalleryApp.Models;
using System.Security.Claims;

namespace GalleryApp.Areas.User.CRUD_GalleryStoredFile
{
    public class CreateModel : PageModel
    {
        private readonly GalleryApp.Data.ApplicationDbContext _context;

        [BindProperty]
        public string UserId { get; set; }

        public CreateModel(GalleryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            UserId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value; // získáme id přihlášeného uživatele
            ViewData["GalleryId"] = new SelectList(_context.Galleries.Where(g => g.CreatorId == UserId), "Id", "Name");
            ViewData["StoredFileId"] = new SelectList(_context.Files, "Id", "OriginalName");
            return Page();
        }

        [BindProperty]
        public GalleryStoredFile GalleryStoredFile { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.GalleryStoredFiles.Add(GalleryStoredFile);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
