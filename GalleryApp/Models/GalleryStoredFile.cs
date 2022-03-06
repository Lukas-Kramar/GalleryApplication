using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalleryApp.Models
{
    public class GalleryStoredFile
    {              
        [Required]        
        [Display(Name = "Gallery")]        
        public int GalleryId { get; set; }
        [ForeignKey("GalleryId")]
        public Gallery Gallery { get; set; }
        [Required]
        [Display(Name = "Picture")] 
        public Guid StoredFileId { get; set; }
        [ForeignKey("StoredFileId")]
        public StoredFile StoredFile { get; set; }
        public string? AltPictureName { get; set; }
    }
}
