using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Itentity.Test
{
    [TestClass]
    public class CreateAssociationsUnitTest : BaseUnitTest
    {
        [TestMethod]
        public void AddRoleToCategory()
        {
            using (var man = Manager)
            {
                var t = DateTime.Now.ToString("HHmmss");
                var cn = "Category_" + t;
                var rn = "Role_" + t;
                var un = "User_" + t;
                object result;

                result = man.CategoryManager.CreateCustomAsync(cn).Result;
                result = man.RoleManager.CreateAsync(rn).Result;

                result = man.RoleManager.AddToCategoryAsync(man.RoleManager.FindByNameAsync(rn).Result.Id, cn).Result;

                if (!man.RoleManager.IsInCategory(man.RoleManager.FindByNameAsync(rn).Result.Id, cn))
                {
                    Assert.Fail("Failed role-category not added.");
                }
            }
        }

        [TestMethod]
        public void AddUserToCategory()
        {
            using (var man = Manager)
            {
                var t = DateTime.Now.ToString("HHmmss");
                var cn = "Category_" + t;
                var rn = "Role_" + t;
                var un = "User_" + t;
                object result;

                result = man.CategoryManager.CreateCustomAsync(cn).Result;
                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;

                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByNameAsync(un).Result.Id, cn).Result;

                if (!man.UserManager.IsInCategory(man.UserManager.FindByNameAsync(un).Result, cn))
                {
                    Assert.Fail("Failed user-category not added.");
                }
            }
        }

        [TestMethod]
        public void AddUserToRole()
        {
            using (var man = Manager)
            {
                var t = DateTime.Now.ToString("HHmmss");
                var cn = "Category_" + t;
                var rn = "Role_" + t;
                var un = "User_" + t;
                object result;

                result = man.RoleManager.CreateAsync(rn).Result;
                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;

                result = man.UserManager.AddToRoleAsync(man.UserManager.FindByNameAsync(un).Result.Id, rn).Result;

                if (!man.UserManager.IsInRoleAsync(man.UserManager.FindByNameAsync(un).Result.Id, rn).Result)
                {
                    Assert.Fail("Failed user-role not added.");
                }
            }
        }
    }
}
