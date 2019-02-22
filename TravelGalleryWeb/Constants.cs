using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace TravelGalleryWeb
{
    public class Constants
    {
        public int PhotoHeight { get; set; }
        public int CoverHeight { get; set; }
        public int PreviewWidth { get; set; }
        public string ResizedPrefix { get; set; }
        public string PreviewPrefix { get; set; }
        public string UploadDir { get; set; }
        public string ResizedDir { get; set; }
        public string PreviewDir { get; set; }
        public string Salt { get; set; }
    }
}