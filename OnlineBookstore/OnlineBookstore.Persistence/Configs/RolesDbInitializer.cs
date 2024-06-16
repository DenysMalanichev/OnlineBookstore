using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineBookstore.Domain.Constants;
using OnlineBookstore.Domain.Entities;

namespace OnlineBookstore.Persistence.Configs;

[ExcludeFromCodeCoverage]
public class RolesDbInitializer
{
    public static async Task SeedRolesToDbAsync(IApplicationBuilder builder)
    {
        using var serviceScope = builder.ApplicationServices.CreateScope();

        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

        await SeedRoleAsync(RoleName.User.ToString(), roleManager);
        await SeedRoleAsync(RoleName.Admin.ToString(), roleManager);
    }

    private static async Task SeedRoleAsync(string roleName, RoleManager<Role> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var role = new Role { Name = roleName };

            await roleManager.CreateAsync(role);
        }
    }
}