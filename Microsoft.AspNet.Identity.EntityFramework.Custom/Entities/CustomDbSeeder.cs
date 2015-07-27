using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Web;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    internal class CustomDbSeeder
    {
        public static void InternalSeed(CustomDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<CustomUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<CustomRoleManager>();
            const string name = "admin@mail.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new CustomRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new CustomUser { UserName = name, Email = name };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }
}
