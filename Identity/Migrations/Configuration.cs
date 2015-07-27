namespace Mvc5.Migrations
{
    using IdentitySample.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Diagnostics;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CustomDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "CustomDbContext";
        }

        protected override void Seed(CustomDbContext context)
        {
           
            var im = new IdentityManager(context);

            var usr = "admin@mail.com";
            var cat = "Administrator";
            IdentityResult result;


            result = im.UserManager.CreateCustomAsync(usr, usr, "password").Result;
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e=>Debug.WriteLine(e));
            }

            result = im.RoleManager.CreateRolesFromEnum<Permissions>().Result;
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => Debug.WriteLine(e));
            }

            result = im.CategoryManager.CreateCustomAsync(cat).Result;
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => Debug.WriteLine(e));
            }

            foreach (var role in im.RoleManager.Roles)
            {
                result = new CustomRoleStore(context).InternalAddToCategoryAsync(context,role.Id, cat).Result;
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(e => Debug.WriteLine(e));
                }
            }
            
            var user = im.UserManager.FindByEmail(usr);
            result = im.UserManager.AddToCategoryAsync(user.Id, cat).Result;
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => Debug.WriteLine(e));
            }

        }
    }
}
