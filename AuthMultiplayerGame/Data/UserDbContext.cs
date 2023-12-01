using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthMultiplayerGame.Data;

// Database context for managing user-related data
public class UserDbContext : IdentityDbContext<IdentityUser>
{
    // Constructor for UserDbContext that takes DbContextOptions as input
    public UserDbContext(DbContextOptions<UserDbContext> options) 
        : base(options) => Database.EnsureCreated();
}
