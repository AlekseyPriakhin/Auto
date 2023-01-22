using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication1.Views.Owners;

public class Create : PageModel
{
    public void OnGet()
    {
        
    }

    public void OnPostOwner()
    {
        Console.WriteLine("I post");
    }
}