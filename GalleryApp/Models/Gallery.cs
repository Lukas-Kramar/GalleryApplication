using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GalleryApp.Models
{
    public class Gallery
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("CreatorId")]
        public AppUser Creator { get; set; }
        [Required]
        public string CreatorId { get; set; }  
        [Required(ErrorMessage = "Needs a name")]
        [MinLength(2, ErrorMessage = "Has to at leas 2 characters")]
        public string Name { get; set; }        
        public string Description { get; set; }
        [Required]
        public ICollection<GalleryStoredFile> StoredPictures { get; set; }
        public int NumberOfPicture { get; set; }
        [Required]
        [Display(Name = "Is public")]
        public bool isPublic { get; set; } = false;        
        [ForeignKey("IdThumbnail")]
        public StoredFile? ThumbnailPicture { get; set; }
        public Guid? IdThumbnail { get; set; }


    }
}
