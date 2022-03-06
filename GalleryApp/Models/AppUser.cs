using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GalleryApp.Models
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "Nickname")]
        public string NickName { get; set; } 
        public ICollection<Gallery> Galleries { get; set; }        
        public int DefaultGalleryId { get; set; }
    }
}
