using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Plunger.Pages.Account;

public class Register : PageModel
{
    private readonly ILogger<Register> _logger;

    public Register(ILogger<Register> logger)
    {
        _logger = logger;
    }

    public void OnGet() { }
}
