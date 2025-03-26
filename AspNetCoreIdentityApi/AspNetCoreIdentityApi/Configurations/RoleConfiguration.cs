using AspNetCoreIdentityApi.Data;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApi.Configurations
{
    public static class RoleConfiguration
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roleExist = await roleManager.RoleExistsAsync("Admin");
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            roleExist = await roleManager.RoleExistsAsync("User");
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }
    }
}
