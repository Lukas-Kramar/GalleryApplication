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
    public class IndexModel : PageModel
    {
        private readonly GalleryApp.Data.ApplicationDbContext _context;

        public IndexModel(GalleryApp.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Gallery> OwnerGalleries { get;set; }
        public IList<Gallery> Galleries { get; set; }
        private string UserId { get; set; }

        public async Task OnGetAsync()
        {

            UserId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;

            OwnerGalleries = await _context.Galleries
                .Include(g => g.Creator)
                .Include(g => g.StoredPictures)
                .Where(g => g.CreatorId == UserId)
                .ToListAsync();
            Galleries = await _context.Galleries
                .Include(g => g.Creator)
                .Include(g => g.StoredPictures)
                .Where(g => g.CreatorId != UserId & g.isPublic == true)
                .ToListAsync();

            foreach (Gallery g in OwnerGalleries)
            {
                g.NumberOfPicture = _context.GalleryStoredFiles.Where(ga => ga.GalleryId == g.Id).Count();
                if(g.NumberOfPicture > 0)
                {
                    Console.WriteLine("Thumbnail ID je:" + g.IdThumbnail.ToString());
                    g.IdThumbnail = (g.IdThumbnail == null) ?
                    _context.Files.Where(f => f.Id == g.StoredPictures.FirstOrDefault().StoredFileId).FirstOrDefault().Id :
                    _context.Files.Where(f => f.Id == g.IdThumbnail).FirstOrDefault().Id;                    
                }               
            }
            foreach (Gallery g in Galleries)
            {
                g.NumberOfPicture = _context.GalleryStoredFiles.Where(ga => ga.GalleryId == g.Id).Count();
                if (g.NumberOfPicture > 0)
                {
                    Console.WriteLine("Thumbnail ID je:" + g.IdThumbnail.ToString());
                    g.IdThumbnail = (g.IdThumbnail == null) ?
                    _context.Files.Where(f => f.Id == g.StoredPictures.FirstOrDefault().StoredFileId).FirstOrDefault().Id :
                    _context.Files.Where(f => f.Id == g.IdThumbnail).FirstOrDefault().Id;
                }
            }
        }
    }
}
