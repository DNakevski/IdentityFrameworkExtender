using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Itentity.Test
{
    [TestClass]
    public class IdentityWebUnitTest : BaseUnitTest
    {
        [TestMethod]
        public void CreateUserAndRole()
        {
            using (var man = Manager)
            {
                var cn = "Administrator";
                var rn = "Home";
                var un = "admin";
                object result;

                result = man.CategoryManager.CreateCustomAsync(cn).Result;
                result = man.RoleManager.CreateCustomAsync(rn).Result;
                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;

                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByName(un).Id, cn).Result;
                result = man.RoleManager.AddToCategoryAsync(man.RoleManager.FindByNameAsync(rn).Result.Id, cn).Result;

                if (!man.UserManager.IsInRoleAsync(man.UserManager.FindByName(un).Id, rn).Result)
                {
                    Assert.Fail("Failed role not removed.");
                }
            }
        }

    }
}
