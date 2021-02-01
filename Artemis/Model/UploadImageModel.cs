using Microsoft.AspNetCore.Http;

namespace Artemis.Model
{
    public class UploadImageModel
    {
        public IFormFile Image { get; set; }
        public string Title { get; set; }
    }
}
