using Microsoft.AspNetCore.Identity;

namespace GalleryApp.Models
{
    public class StoredFileListViewModel
    {
        public Guid Id { get; set; }
        public AppUser Uploader { get; set; }
        public string UploaderId { get; set; }
        public DateTime UploadedAt { get; set; }
        public string OriginalName { get; set; }
        public string ContentType { get; set; }
        public int ThumbnailCount { get; set; }
    }
}
