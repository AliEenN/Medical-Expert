using MedicalExpert.Constants;
using MedicalExpert.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalExpert.Models
{
    public class SeedingData
    {
        private ApplicationDbContext _context;

        public SeedingData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async void SeedAdminUser()
        {
            var user = new ApplicationUser
            {
                Id = "164d633f-c87a-42ef-ada8-142256bd54e9",
                UserName = "ali.nasser.9.1997@gmail.com",
                NormalizedUserName = "ALI.NASSER.9.1997@GMAIL.COM",
                Email = "ali.nasser.9.1997@gmail.com",
                NormalizedEmail = "ALI.NASSER.9.1997@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = "AQAAAAEAACcQAAAAELH/5/Lc/L9C2NRodOWyyacXumgzwWHnqBCkzsboQ52TtT2j5SgpbkYt+Cx6QrvbCw==",
                SecurityStamp = "FBX4JWID5VXCSPGFVFRGG2X7KZC2OFND",
                ConcurrencyStamp = "8d8d1c60-048d-484e-9e9a-4e46a87d3a6f",
                LockoutEnabled = true
            };

            var roleStore = new RoleStore<IdentityRole>(_context);

            if (!_context.Roles.Any(e => e.Name == Roles.Admin))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = Roles.Admin, NormalizedName = Roles.Admin.ToUpper() });
            }

            if (!_context.Roles.Any(e => e.Name == Roles.Manager))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = Roles.Manager, NormalizedName = Roles.Manager.ToUpper() });
            }

            if (!_context.Roles.Any(e => e.Name == Roles.Doctor))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = Roles.Doctor, NormalizedName = Roles.Doctor.ToUpper() });
            }

            if (!_context.Roles.Any(e => e.Name == Roles.User))
            {
                await roleStore.CreateAsync(new IdentityRole { Name = Roles.User, NormalizedName = Roles.User.ToUpper() });
            }

            if (!_context.Users.Any(e => e.UserName == user.UserName))
            {
                var userStore = new UserStore<IdentityUser>(_context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "Admin");
            }

            await _context.SaveChangesAsync();
        }
    }
}
