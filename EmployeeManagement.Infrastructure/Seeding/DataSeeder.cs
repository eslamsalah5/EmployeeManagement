using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagement.Infrastructure.Seeding;

public class DataSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public DataSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        // Seed Admin Role
        var adminSettings = _configuration.GetSection("AdminSettings");
        var roleName = adminSettings["Role"]!;

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Seed Admin User
        var adminEmail = adminSettings["Email"]!;
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var password = adminSettings["Password"]!;
            var result = await _userManager.CreateAsync(adminUser, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, roleName);
            }
        }
    }
}
