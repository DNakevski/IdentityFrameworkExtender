using System.Threading.Tasks;
using System.Linq;
using System;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomUserStore : UserStore<CustomUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        #region // <FieldsAndProperties>

        private CustomCategoryManager categoryManager;
        public CustomCategoryManager CategoryManager
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

        #endregion // </FieldsAndProperties>

        public CustomUserStore(CustomDbContext context)
            : base(context)
        {
        }

        public virtual bool IsInCategory(int userId, string categoryName)
        {
            var user = FindByIdAsync(userId).Result;
            return IsInCategory(user, categoryName);
        }
        public virtual bool IsInCategory(CustomUser user, string categoryName)
        {
            return user.CategoryUsers.Select(cu => cu.Category).Any(c => c.Name == categoryName);
        }

        /// <summary>
        /// Remove other categories if present
        /// </summary>
        public virtual async Task<IdentityResult> AddToCategoryAsync(int userId, string name)
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
                        if (!await CategoryManager.CategoryExistsAsync(name))
                        {
                            returnValue = IdentityResult.Failed("Category " + name + " does not exists!");
                            tran.Rollback();
                            return returnValue;
                        }

                        var user = await FindByIdAsync(userId);
                        if (user == null)
                        {
                            returnValue = IdentityResult.Failed("User with id " + userId + " is not single!");
                            tran.Rollback();
                            return returnValue;
                        }

                        if (IsInCategory(user, name))
                        {
                            returnValue = IdentityResult.Failed("User is already in category " + name + "!");
                            tran.Rollback();
                            return returnValue;
                        }

                        var category = await CategoryManager.FindByNameAsync(name);
                        if (category == null)
                        {
                            returnValue = IdentityResult.Failed("Category " + name + " is not single!");
                            tran.Rollback();
                            return returnValue;
                        }

                        user.CategoryUsers.Clear();
                        await Context.SaveChangesAsync();

                        context.CategoryUsers.Add(new CustomCategoryUser { UserId = user.Id, CategoryId = category.Id });
                        await Context.SaveChangesAsync();

                        user.Roles.Clear();
                        await Context.SaveChangesAsync();

                        if (category.CategoryRoles.Any())
                        {
                            foreach (var cr in category.CategoryRoles)
                            {
                                await AddToRoleAsync(user, cr.Role.Name);
                            }
                        }

                        await Context.SaveChangesAsync();

                        returnValue = IdentityResult.Success;

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
        public virtual async Task<IdentityResult> RemoveFromCategoryAsync(int userId, string name)
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
                        if (!await CategoryManager.CategoryExistsAsync(name))
                        {
                            returnValue = IdentityResult.Failed("Category " + name + " does not exists!");
                            tran.Rollback();
                            return returnValue;
                        }

                        var user = await FindByIdAsync(userId);
                        if (user == null)
                        {
                            returnValue = IdentityResult.Failed("User with id " + userId + " is not single!");
                            tran.Rollback();
                            return returnValue;
                        }

                        if (!IsInCategory(user, name))
                        {
                            returnValue = IdentityResult.Failed("User is not in category " + name + "!");
                            tran.Rollback();
                            return returnValue;
                        }

                        var category = await CategoryManager.FindByNameAsync(name);
                        if (category == null)
                        {
                            returnValue = IdentityResult.Failed("Category " + name + " is not single!");
                            tran.Rollback();
                            return returnValue;
                        }

                        var categoryUsers = context
                            .CategoryUsers
                            .Where(cr => cr.CategoryId == category.Id)
                            .Where(cr => cr.UserId == user.Id);

                        context.CategoryUsers.RemoveRange(categoryUsers);
                        await Context.SaveChangesAsync();

                        user.Roles.Clear();
                        await Context.SaveChangesAsync();

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
            }
        }
    }
}