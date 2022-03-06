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
    public class IndexModel : PageModel
    {
        private readonly GalleryApp.Data.ApplicationDbContext _context;

        public IndexModel(GalleryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<GalleryStoredFile> GalleryStoredFile { get;set; }

        public async Task OnGetAsync()
        {
            GalleryStoredFile = await _context.GalleryStoredFiles
                .Include(g => g.Gallery)
                .Include(g => g.StoredFile).ToListAsync();
        }
    }
}
