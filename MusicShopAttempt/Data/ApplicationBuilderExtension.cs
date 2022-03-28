using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicShopAttempt.Data
{
    public static class ApplicationBuilderExtension
    {
        public static async Task<IApplicationBuilder> PrepareDataBase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await SeedRolesAsync(roleManager);
                await SeedSuperAdminAsync(userManager);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred seeding the DB.");
            }

            return app;
        }
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.User.ToString()));
        }

        public static async Task SeedSuperAdminAsync(UserManager<User> userManager)
        {
            var defaultUser = new User
            {
                UserName = "admin@gmail.com",
                FirstName = "Polly",
                Surname = "Andonova",
                PhoneNumber = "0889713925",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(defaultUser.UserName);
            if (user == null)
            {
                var result = await userManager.CreateAsync(defaultUser, "123456Pla*");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, Role.Admin.ToString());                 
                }                
            }
        }
    }
}
