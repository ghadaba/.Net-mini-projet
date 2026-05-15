using Microsoft.AspNetCore.Identity;
using MiniProjet.Constants;

namespace MiniProjet.data
{
    public static class DbInitializer
    {
        public const string DefaultAdminUserName = "admin";
        public const string DefaultAdminPassword = "Admin123!";

        public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var roleName in new[] { RoleNames.Admin, RoleNames.User })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var admin = await userManager.FindByNameAsync(DefaultAdminUserName);
            if (admin == null)
            {
                admin = new IdentityUser { UserName = DefaultAdminUserName, Email = "admin@local.dev" };
                var result = await userManager.CreateAsync(admin, DefaultAdminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, RoleNames.Admin);
            }
            else if (!await userManager.IsInRoleAsync(admin, RoleNames.Admin))
            {
                await userManager.AddToRoleAsync(admin, RoleNames.Admin);
            }
        }
    }
}
