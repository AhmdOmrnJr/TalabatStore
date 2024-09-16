using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> _userManager)
        {
            if (_userManager.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName = "Ahmed Omran",
                    Email = "aomran@gmail.com",
                    UserName = "ahmed.omran",
                    PhoneNumber = "0123456789"
                };
                
                await _userManager.CreateAsync(user, "Password123!");
            }
        }
    }
}
