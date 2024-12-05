using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Plunger.Pages.Account;

[Authorize]
public class Index : PageModel
{
    // public async Task<IActionResult> OnPostAsync()
    // {
    //     return new PageResult();
    // }
}
