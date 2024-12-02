using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Plunger.Data;

namespace Plunger.Pages.Account;

public class Login : PageModel
{
    private readonly SignInManager<PlungerUser> _signInManager;
    private readonly UserManager<PlungerUser> _userManager;

    public Login(SignInManager<PlungerUser> signInManager, UserManager<PlungerUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
}
