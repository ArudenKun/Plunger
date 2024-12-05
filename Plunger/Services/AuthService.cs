using AutoInterfaceAttributes;
using Microsoft.AspNetCore.Identity;
using Plunger.Data;

namespace Plunger.Services;

[AutoInterface]
public class AuthService : IAuthService
{
    private readonly SignInManager<PlungerUser> _signInManager;

    public AuthService(SignInManager<PlungerUser> signInManager)
    {
        _signInManager = signInManager;
    }
}
