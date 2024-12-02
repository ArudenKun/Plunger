using Microsoft.AspNetCore.Identity;

namespace Plunger.Data;

public class PlungerUser : IdentityUser
{
    public string GuildName { get; set; } = string.Empty;

    public PlungerUser(string userName)
        : base(userName) { }
}
