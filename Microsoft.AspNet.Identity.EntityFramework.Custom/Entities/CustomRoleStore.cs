using System.Threading.Tasks;
using System.Linq;
using System;
using System.Data.Entity;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        #region // <FieldsAndProperties>

        private CustomCategoryManager categoryManager;
        protected CustomCategoryManager CategoryManager
        {
            get
            {
                if (categoryManager == null)
                {
                    categoryManager = new CustomCategoryManager(new CustomCategoryStore(Context as CustomDbContext));
                }

                return categoryManager;
            }
        }

        private CustomUserManager userManager;
        protected CustomUserManager UserManager
        {
            get
            {
                if (userManager == null)
                {
                    userManager = new CustomUserManager(new CustomUserStore(Context as CustomDbContext));
                }

                return userManager;
            }
        }

        #endregion // </FieldsAndProperties>

        public CustomRoleStore(CustomDbContext context)
            : base(context)
        {
        }

        public virtual async Task<bool> IsInCategoryAsync(int roleId, string categoryName)
        {
            return await (Context as CustomDbContext)
                .Categories
                .Where(c => c.Name == categoryName)
                .SelectMany(c => c.CategoryRoles)
                .AnyAsync(r => r.RoleId == roleId);
        }
        public virtual bool IsInCategory(int roleId, string categoryName)
        {
            return (Context as CustomDbContext)
                .Categories
                .Where(c => c.Name == categoryName)
                .SelectMany(c => c.CategoryRoles)
                .Any(r => r.RoleId == roleId);
        }
        public virtual async Task<IdentityResult> AddToCategoryAsync(int roleId, string name)
        {
            IdentityResult returnValue = IdentityResult.Success;

            var context = Context as CustomDbContext;
            if (context == null)
            {
                returnValue = IdentityResult.Failed("Context is not a CustomDbContext!");
            }
            else
            {
                using (var tran = context.Database.BeginTransaction())
                {
                    returnValue = await InternalAddToCategoryAsync(context, roleId, name);
                    if (returnValue.Succeeded)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }
                }
            }

            return returnValue;
        }
        public virtual async Task<IdentityResult> RemoveFromCategoryAsync(int roleId, string name)
        {
            IdentityResult returnValue = IdentityResult.Success;

            var context = Context as CustomDbContext;
            if (context == null)
            {
                returnValue = IdentityResult.Failed("Context is not a CustomDbContext!");
            }
            else
            {
                using (var tran = context.Database.BeginTransaction())
                {
                    returnValue = await InternalRemoveFromCategoryAsync(context, roleId, name);
                    if (returnValue.Succeeded)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }
                }
            }

            return returnValue;
        }

        public virtual async Task<IdentityResult> AddToCategoryAndRemoveTheOthersAsync(int[] roleIds, string name)
        {
            IdentityResult returnValue = IdentityResult.Success;

            var context = Context as CustomDbContext;
            if (context == null)
            {
                returnValue = IdentityResult.Failed("Context is not a CustomDbContext!");
            }
            else
            {
                using (var tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        var category = await CategoryManager.FindByNameAsync(name);
                        var categoryRoles = category.CategoryRoles.Select(c => c.Role).Select(r => r.Id).ToList();

                        foreach (var roleId in roleIds.Except(categoryRoles))
                        {
                            var addResult = await InternalAddToCategoryAsync(context, roleId, name);
                            if (!addResult.Succeeded)
                            {
                                returnValue = addResult;
                                tran.Rollback();
                                return returnValue;
                            }
                        }

                        foreach (var roleId in categoryRoles.Except(roleIds))
                        {
                            var removeResult = await InternalRemoveFromCategoryAsync(context, roleId, name);
                            if (!removeResult.Succeeded)
                            {
                                returnValue = removeResult;
                                tran.Rollback();
                                return returnValue;
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        returnValue = IdentityResult.Failed("Internal Server Error : " + ex.ToString());
                        tran.Rollback();
                    }
                }
            }

            return returnValue;
        }

        public async Task<IdentityResult> InternalRemoveFromCategoryAsync(CustomDbContext context, int roleId, string name)
        {
            IdentityResult returnValue = IdentityResult.Success;

            try
            {
                if (!await CategoryManager.CategoryExistsAsync(name))
                {
                    returnValue = IdentityResult.Failed("Category " + name + " does not exists!");
                    return returnValue;
                }

                var role = await FindByIdAsync(roleId);
                if (role == null)
                {
                    returnValue = IdentityResult.Failed("Role with id " + roleId + " is not single!");
                    return returnValue;
                }

                if (!IsInCategory(role.Id, name))
                {
                    returnValue = IdentityResult.Failed("Role is not in category " + name + "!");
                    return returnValue;
                }

                var category = await CategoryManager.FindByNameAsync(name);
                if (category == null)
                {
                    returnValue = IdentityResult.Failed("Category " + name + " is not single!");
                    return returnValue;
                }

                var categoryRoles = context
                    .CategoryRoles
                    .Where(cr => cr.CategoryId == category.Id)
                    .Where(cr => cr.RoleId == role.Id);
                context.CategoryRoles.RemoveRange(categoryRoles);
                await Context.SaveChangesAsync();

                var users = category.CategoryUsers.Select(u => u.User.Id);
                foreach (var userId in users)
                {
                    var inRole = await UserManager.IsInRoleAsync(userId, role.Name);
                    if (inRole)
                    {
                        var removeResult = await UserManager.RemoveFromRolesAsync(userId, role.Name);
                        if (!removeResult.Succeeded)
                        {
                            returnValue = removeResult;
                            return returnValue;
                        }
                    }
                }

                await Context.SaveChangesAsync();
                returnValue = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                returnValue = IdentityResult.Failed("Internal Server Error : " + ex.ToString());
            }

            return returnValue;
        }
        public async Task<IdentityResult> InternalAddToCategoryAsync(CustomDbContext context, int roleId, string name)
        {
            IdentityResult returnValue = IdentityResult.Success;

            try
            {
                if (!await CategoryManager.CategoryExistsAsync(name))
                {
                    returnValue = IdentityResult.Failed("Category " + name + " does not exists!");
                    return returnValue;
                }

                var role = await FindByIdAsync(roleId);
                if (role == null)
                {
                    returnValue = IdentityResult.Failed("Role with id " + roleId + " is not single!");
                    return returnValue;
                }

                if (IsInCategory(role.Id, name))
                {
                    returnValue = IdentityResult.Failed("Role already in category " + name + "!");
                    return returnValue;
                }

                var category = await CategoryManager.FindByNameAsync(name);
                if (category == null)
                {
                    returnValue = IdentityResult.Failed("Category " + name + " is not single!");
                    return returnValue;
                }

                context.CategoryRoles.Add(new CustomCategoryRole { RoleId = role.Id, CategoryId = category.Id });
                await Context.SaveChangesAsync();

                var users = category.CategoryUsers.Select(u => u.User.Id);
                foreach (var userId in users)
                {
                    var inRole = await UserManager.IsInRoleAsync(userId, role.Name);
                    if (!inRole)
                    {
                        var addResult = await UserManager.AddToRoleAsync(userId, role.Name);
                        if (!addResult.Succeeded)
                        {
                            returnValue = addResult;
                            return returnValue;
                        }
                    }
                }

                await Context.SaveChangesAsync();

                returnValue = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                returnValue = IdentityResult.Failed("Internal Server Error : " + ex.ToString());
            }

            return returnValue;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                // dispose internal field (The property is never null)
                if (categoryManager != null)
                {
                    categoryManager.Dispose();
                }

                // dispose internal field (The property is never null)
                if (userManager != null)
                {
                    userManager.Dispose();
                }
            }
        }
    }
}