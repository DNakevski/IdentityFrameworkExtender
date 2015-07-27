using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomCategoryStore : IDisposable
    {
        #region // <FieldsAndProperties>

        public CustomDbContext Context { get; private set; }

        protected bool disposed = false;

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

        #region // <Constructors>

        public CustomCategoryStore(CustomDbContext context)
        {
            this.Context = context;
        }

        #endregion // </Constructors>

        #region // <Dispose>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (Context != null)
                {
                    Context.Dispose();
                }
            }

            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion // </Dispose>

        internal async Task<IdentityResult> CreateAsync(string name, string description, int? code)
        {
            if (name == null)
            {
                return IdentityResult.Failed("Category name is null");
            }

            if (await CategoryExistsAsync(name))
            {
                return IdentityResult.Failed("Category with that name already exists!");
            }

            var c = new CustomCategory();
            c.Name = name;

            if (description != null)
            {
                c.Description = description;
            }

            if (code.HasValue)
            {
                c.TranslationCode = code.Value;
            }

            Context.Categories.Add(c);

            var result = await Context.SaveChangesAsync();
            if (result > 0)
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed("Internal Server Error");
            }
        }

        internal async Task<IdentityResult> DeleteAsync(string name)
        {
            IdentityResult returnValue = IdentityResult.Success;

            if (name == null)
            {
                return IdentityResult.Failed("Category name is null");
            }

            var context = Context;
            if (context == null)
            {
                return IdentityResult.Failed("Null CustomDbContext");
            }
            else
            {
                using (DbContextTransaction tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        var category = await FindByNameAsync(name);
                        if (category == null)
                        {
                            returnValue = IdentityResult.Failed("Category with that name does not exists!");
                            tran.Rollback();
                            return returnValue;
                        }

                        if (category.IsDeleted)
                        {
                            tran.Commit();
                            return returnValue;
                        }

                        // Remove user role
                        var categoryRoles = context.CategoryRoles.Where(cr => cr.Category.Id == category.Id).Select(cr => cr.Role.Id);
                        var categoryUsers = context.CategoryUsers.Where(cu => cu.Category.Id == category.Id).Select(cr => cr.User.Id);
                        var userRoles = from ur in context.UserRoles
                                        join c in categoryRoles on ur.RoleId equals c
                                        join u in categoryUsers on ur.UserId equals u
                                        select ur;
                        context.UserRoles.RemoveRange(userRoles);
                        await context.SaveChangesAsync();

                        // Clear users
                        category.CategoryUsers.Clear();
                        await context.SaveChangesAsync();

                        // Clear roles
                        category.CategoryRoles.Clear();
                        await context.SaveChangesAsync();

                        category.IsDeleted = true;
                        category.DeletedOn = DateTime.Now;
                        category.Name = string.Empty;
                        await context.SaveChangesAsync();

                        tran.Commit();

                        return IdentityResult.Success;
                    }
                    catch (Exception ex)
                    {
                        returnValue = IdentityResult.Failed("Internal Server Error : " + ex.ToString());
                        tran.Rollback();
                        return returnValue;
                    }
                }
            }
        }

        internal async Task<IdentityResult> UpdateAsync(int categoryId, string name, string description, int? code)
        {
            try
            {
                var c = await FindByIdAsync(categoryId);

                if (c.Name != name)
                {
                    if (await CategoryExistsAsync(name))
                    {
                        return IdentityResult.Failed("Category with that name already exists!");
                    }
                    else
                    {
                        c.Name = name;
                    }
                }

                if (description != null)
                {
                    c.Description = description;
                }

                if (code.HasValue)
                {
                    c.TranslationCode = code.Value;
                }

                await Context.SaveChangesAsync();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed("Internal Server Error : " + ex.ToString());
            }
        }

        internal async Task<bool> CategoryExistsAsync(string categoryName)
        {
            return await Context
                .Categories
                .AnyAsync(c => c.Name == categoryName);
        }

        internal async Task<CustomCategory> FindByNameAsync(string categoryName)
        {
            return await Context
                .Categories
                .SingleOrDefaultAsync(c => c.Name == categoryName);
        }

        internal async Task<CustomCategory> FindByIdAsync(int idCategory)
        {
            return await Context
                .Categories
                .SingleOrDefaultAsync(c => c.Id == idCategory);
        }
    }
}
