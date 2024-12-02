using Microsoft.AspNetCore.Identity;
using Plunger.Data;

namespace Plunger.Services;

public class AuthService
{
    private readonly SignInManager<PlungerUser> _signInManager;

    public AuthService(SignInManager<PlungerUser> signInManager)
    {
        _signInManager = signInManager;
    }
}
