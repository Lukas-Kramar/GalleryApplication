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
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace GalleryApp.Areas.User.CRUD_Gallery
{
    public class CreateModel : PageModel
    {
        private readonly GalleryApp.Data.ApplicationDbContext _context;

        public CreateModel(GalleryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public string UserId { get; set; }

        public IActionResult OnGet()
        {
            UserId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
            return Page();
        }

        [BindProperty]
        public Gallery Gallery { get; set; }
       

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            Gallery.StoredPictures = new List<GalleryStoredFile> { };            
            //if (!ModelState.IsValid)
            //{
            //    Console.WriteLine("Not Valid");
            //    return Page();
            //}                        
            _context.Galleries.Add(Gallery);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
