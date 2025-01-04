using Core.Constants;
using Core.Entities;

using Microsoft.AspNetCore.Identity;

namespace IdentityProject
{
    public static class DbInitializer
    {
        public static async Task Seed(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
           await AddRoles(roleManager);
           await AddAdmin(userManager, roleManager);
        }
        private static async Task AddRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetValues<UserRoles>())
            {
                if (!await roleManager.RoleExistsAsync(role.ToString()))
                {
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString(),
                    });
                }
            }
        }
        private static async Task AddAdmin(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {

            if (await userManager.FindByEmailAsync("admin@app.com") is null)
            {

                var user = new User
                {
                    UserName = "admin@app.com",
                    Email = "admin@app.com",
                    Country = "Azerbaycan",
                    City = "Baku"
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (!result.Succeeded)
                    throw new Exception("Could not add admin");

                var role = await roleManager.FindByNameAsync("Admin");
                if (role?.Name is null)
                    throw new Exception("Could not find admin role");

                var addToResult = await userManager.AddToRoleAsync(user, role.Name);
                if (!addToResult.Succeeded)
                    throw new Exception("It was not possible to add the admin role to the user");
            }
        }
    }
}
