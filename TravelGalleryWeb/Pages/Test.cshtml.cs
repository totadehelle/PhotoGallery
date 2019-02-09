using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TravelGalleryWeb.Pages
{
    public class TestModel : PageModel
    {
        public string Name = "Mary";
        
        public void OnGet()
        {
            if(HttpContext.User.Identity.Name != null)
            {
                Name = HttpContext.User.Identity.Name;
            }
            
        }

        public void OnPost()
        {
            if(HttpContext.User.Identity.Name != null)
            {
                Name = HttpContext.User.Identity.Name;
            }
            else{Name = "John";}
        }
    }
}