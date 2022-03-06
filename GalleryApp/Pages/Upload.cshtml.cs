using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using GalleryApp.Data;
using GalleryApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace GalleryApp.Pages
{
    public class UploadModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private ApplicationDbContext _context;
        private IConfiguration _configuration;
        private int _squareSize;
        private int _sameAspectRatioHeigth;
        private int _maxWidth;

        [TempData]
        public string SuccessMessage { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

        public ICollection<IFormFile> Upload { get; set; }
        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public GalleryStoredFile GalleryStoredFile { get; set; }        
        public Gallery Gallery { get; set; }

        public UploadModel(IWebHostEnvironment environment, ApplicationDbContext context, IConfiguration configuration)
        {
            _environment = environment;
            _context = context;
            _configuration = configuration;
            if (Int32.TryParse(_configuration["Thumbnails:SquareSize"], out _squareSize) == false) _squareSize = 64; // získej data z konfigurave nebo použij 64
            if (Int32.TryParse(_configuration["Thumbnails:SameAspectRatioHeigth"], out _sameAspectRatioHeigth) == false) _sameAspectRatioHeigth = 128;
            if (Int32.TryParse(_configuration["UploadFile:MaxWidth"], out _maxWidth) == false) _maxWidth = 2000;
        }

        public void OnGet()
        {
            UserId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value.ToString(); // získáme id pøihlášeného uživatele
            ViewData["Gallery"] = new SelectList(_context.Galleries.Where(g => g.CreatorId == UserId), "Id", "Name");
        }
    public async Task<IActionResult> OnPostAsync()
    {
        int successfulProcessing = 0;
        int failedProcessing = 0;
        foreach (var uploadedFile in Upload)
        {                       
            var fileRecord = new StoredFile
            {
                OriginalName = uploadedFile.FileName,
                UploaderId = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value.ToString(),
                UploadedAt = DateTime.Now,
                ContentType = uploadedFile.ContentType,
                Galleries = new List<GalleryStoredFile> { }
            };
            if (_context.Galleries.FirstOrDefault(g => g.Id == GalleryStoredFile.GalleryId).isPublic == true) fileRecord.isPublic = true;
            if (uploadedFile.ContentType.StartsWith("image")) // je soubor obrázek?
            {                
                fileRecord.Thumbnails = new List<Thumbnail>();
                MemoryStream ims = new MemoryStream(); // proud pro pøíchozí obrázek
                MemoryStream oms1 = new MemoryStream(); // proud pro ètvercový náhled
                MemoryStream oms2 = new MemoryStream(); // proud pro obdélníkový náhled
                uploadedFile.CopyTo(ims); // vlož obsah do vstupního proudu                
                IImageFormat format; // zde si uložíme formát obrázku (JPEG, GIF, ...), budeme ho potøebovat pøi ukládání
                using (Image image = Image.Load(ims.ToArray(), out format)) // vytvoøíme ètvercový náhled
                {
                if(image.Width > _maxWidth)
                    {
                        ErrorMessage = $"Width of the picture cant surpase {_maxWidth}px!";
                        return RedirectToPage("/Index");
                    }
                    int largestSize = Math.Max(image.Height, image.Width); // jaká je orientace obrázku?
                    if (image.Width > image.Height) // podle orientace zmìníme velikost obrázku
                    {
                        image.Mutate(x => x.Resize(0, _squareSize));
                    }
                    else
                    {
                        image.Mutate(x => x.Resize(_squareSize, 0));
                    }
                    image.Mutate(x => x.Crop(new Rectangle((image.Width - _squareSize) / 2, (image.Height - _squareSize) / 2, _squareSize, _squareSize)));
                    // obrázek oøízneme na ètverec
                    image.Save(oms1, format); // vložíme ho do výstupního proudu
                    fileRecord.Thumbnails.Add(new Thumbnail { Type = ThumbnailType.Square, Blob = oms1.ToArray() }); // a uložíme do databáze jako pole bytù
                }
                using (Image image = Image.Load(ims.ToArray(), out format)) // obdélníkový náhled zaèíná zde
                {
                    image.Mutate(x => x.Resize(0, _sameAspectRatioHeigth)); // staèí jen zmìnit jeho velikost
                    image.Save(oms2, format); // a pøes proud ho uložit do databáze
                    fileRecord.Thumbnails.Add(new Thumbnail { Type = ThumbnailType.SameAspectRatio, Blob = oms2.ToArray() });
                }
            }
            try
            {
                _context.Files.Add(fileRecord); // a uložíme ho
                await _context.SaveChangesAsync(); // tím se nám vygeneruje jeho klíè ve formátu Guid

                GalleryStoredFile.StoredFileId = fileRecord.Id; //StoredFile se naváže na gallerii, kteár byla vybrána
                _context.GalleryStoredFiles.Add(GalleryStoredFile);
                await _context.SaveChangesAsync();

                
                Gallery = _context.Galleries.Where(g => g.Id == GalleryStoredFile.GalleryId).FirstOrDefault();
                if (Gallery.Name != "Default Gallery")
                {
                    Console.WriteLine(_context.Galleries.Where(g => g.CreatorId == Gallery.CreatorId).Where(g => g.Name == "Default Gallery").FirstOrDefault().Id.ToString());
                    _context.GalleryStoredFiles.Add(new GalleryStoredFile
                    {
                        GalleryId = _context.Galleries.Where(g => g.CreatorId == Gallery.CreatorId).Where(g => g.Name == "Default Gallery").FirstOrDefault().Id,
                        StoredFileId = fileRecord.Id
                    });
                }
                await _context.SaveChangesAsync();
                var file = Path.Combine(_environment.ContentRootPath, "Uploads", fileRecord.Id.ToString());
                // pod tímto klíèem uložíme soubor i fyzicky na disk
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                };
                successfulProcessing++;
            }
            catch
            {
                failedProcessing++;
            }
        }
        if (failedProcessing == 0)
        {
            SuccessMessage = "All files has been uploaded successfuly.";
        }
        else
        {
            ErrorMessage = "There were " + failedProcessing + " errors during uploading and processing of files.";
        }
        return RedirectToPage("/Index");
        }
    }
}