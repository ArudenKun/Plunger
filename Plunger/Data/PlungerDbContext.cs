using Microsoft.EntityFrameworkCore;

namespace Plunger.Data;

public class PlungerDbContext : DbContext
{
    public PlungerDbContext(DbContextOptions<PlungerDbContext> options)
        : base(options) { }
}
