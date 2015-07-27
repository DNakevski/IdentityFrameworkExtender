using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Itentity.Test
{
    [TestClass]
    public class ComplexUnitTest : BaseUnitTest
    {
        [TestMethod]
        public void AddRoleToCategory_Should_AddRoleToUsersThatHaveThatCategory()
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
                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;

                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByName(un).Id, cn).Result;
                result = man.RoleManager.AddToCategoryAsync(man.RoleManager.FindByNameAsync(rn).Result.Id, cn).Result;

                if (!man.UserManager.IsInRoleAsync(man.UserManager.FindByName(un).Id, rn).Result)
                {
                    Assert.Fail("Failed role not removed.");
                }
            }
        }

        [TestMethod]
        public void AddUserToCategory_Should_AddRolesOfCategoryToTheUser()
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
                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;

                result = man.RoleManager.AddToCategoryAsync(man.RoleManager.FindByNameAsync(rn).Result.Id, cn).Result;
                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByName(un).Id, cn).Result;

                if (!man.UserManager.IsInRoleAsync(man.UserManager.FindByName(un).Id, rn).Result)
                {
                    Assert.Fail("Failed role not added.");
                }
            }
        }

        [TestMethod]
        public void RemoveRoleFromCategory_Should_RemoveRoleFromTheUserThatHaveThatCategory()
        {
            using (var man = Manager)
            {
                var t = DateTime.Now.ToString("HHmmss");
                var cn = "Category_" + t;
                var rn = "Role_" + t;
                var un = "User_" + t;
                var un2 = "User2_" + t;
                object result;

                result = man.CategoryManager.CreateCustomAsync(cn).Result;
                result = man.RoleManager.CreateAsync(rn).Result;
                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;
                result = man.UserManager.CreateCustomAsync(un2, un2 + "@" + un2 + ".com", "password").Result;

                result = man.RoleManager.AddToCategoryAsync(man.RoleManager.FindByNameAsync(rn).Result.Id, cn).Result;
                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByName(un).Id, cn).Result;
                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByName(un2).Id, cn).Result;

                result = man.RoleManager.RemoveFromCategoryAsync(man.RoleManager.FindByNameAsync(rn).Result.Id, cn).Result;
                if (man.UserManager.IsInRoleAsync(man.UserManager.FindByName(un).Id, rn).Result)
                {
                    Assert.Fail("Failed role not removed.");
                }

                if (man.UserManager.IsInRoleAsync(man.UserManager.FindByName(un2).Id, rn).Result)
                {
                    Assert.Fail("Failed role not removed from 2.");
                }
            }
        }

        [TestMethod]
        public void RemoveUserFromCategory_Should_RemoveRolesOfCategoryFromThatUser()
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
                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;

                result = man.RoleManager.AddToCategoryAsync(man.RoleManager.FindByNameAsync(rn).Result.Id, cn).Result;
                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByName(un).Id, cn).Result;

                result = man.UserManager.RemoveFromCategoryAsync(man.UserManager.FindByName(un).Id, cn).Result;
                if (man.UserManager.IsInRoleAsync(man.UserManager.FindByName(un).Id, rn).Result)
                {
                    Assert.Fail("Failed role not removed.");
                }
            }
        }


        [TestMethod]
        public void AddUserToCategory_Should_RemoveOtherCategoriesToTheUser()
        {
            using (var man = Manager)
            {
                var t = DateTime.Now.ToString("HHmmss");
                var cn = "Category_" + t;
                var cn2 = "Category2_" + t;
                var rn = "Role_" + t;
                var rn2 = "Role2_" + t;
                var un = "User_" + t;
                object result;

                result = man.CategoryManager.CreateCustomAsync(cn).Result;
                result = man.RoleManager.CreateAsync(rn).Result;
                result = man.CategoryManager.CreateCustomAsync(cn2).Result;
                result = man.RoleManager.CreateAsync(rn2).Result;
                result = man.UserManager.CreateCustomAsync(un, un + "@" + un + ".com", "password").Result;

                result = man.RoleManager.AddToCategoryAsync(man.RoleManager.FindByNameAsync(rn).Result.Id, cn).Result;
                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByName(un).Id, cn).Result;

                result = man.RoleManager.AddToCategoryAsync(man.RoleManager.FindByNameAsync(rn2).Result.Id, cn2).Result;
                result = man.UserManager.AddToCategoryAsync(man.UserManager.FindByName(un).Id, cn2).Result;

                if (man.UserManager.IsInCategory(man.UserManager.FindByName(un), cn))
                {
                    Assert.Fail("Failed category not removed.");
                }
            }
        }
    }
}
