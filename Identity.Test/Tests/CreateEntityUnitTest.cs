using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Itentity.Test
{
    [TestClass]
    public class CreateEntityUnitTest : BaseUnitTest
    {
        [TestMethod]
        public void CreateUser()
        {
            using (var man = Manager)
            {
                var t = DateTime.Now.ToString("HHmmss");
                var un = "User_" + t;
                object result;

                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;

                if (man.UserManager.FindByName(un).Id == null)
                {
                    Assert.Fail("Failed to create user.");
                }
            }
        }

        [TestMethod]
        public void CreateCategory()
        {
            using (var man = Manager)
            {
                var t = DateTime.Now.ToString("HHmmss");
                var cn = "Category_" + t;
                object result;

                result = man.CategoryManager.CreateCustomAsync(cn).Result;

                if (man.CategoryManager.FindByNameAsync(cn).Result == null)
                {
                    Assert.Fail("Failed to create category.");
                }
            }
        }

        [TestMethod]
        public void CreateRole()
        {
            using (var man = Manager)
            {
                var t = DateTime.Now.ToString("HHmmss");
                var rn = "Role_" + t;
                object result;

                result = man.RoleManager.CreateAsync(new CustomRole { Name = rn }).Result;

                if (man.RoleManager.FindByNameAsync(rn).Result == null)
                {
                    Assert.Fail("Failed to create role.");
                }
            }
        }
    }
}
