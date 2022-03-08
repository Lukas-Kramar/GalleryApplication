using GalleryApp.Data;
using GalleryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GalleryApp.Areas.User.Pages.CRUD_StoredFile
{
    public class ViewFullSizeModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;

        public ViewFullSizeModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
        }

        [BindProperty]
        public StoredFile StoredFile { get; set; }
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

            return Page();
        }
    }
}
