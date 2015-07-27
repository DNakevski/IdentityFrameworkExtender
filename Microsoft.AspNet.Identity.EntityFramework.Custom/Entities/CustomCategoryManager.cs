using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomCategoryManager : IDisposable
    {
        #region // <FiledsAndProperties>

        public IQueryable<CustomCategory> Categories
        {
            get
            {
                return Store.Context.Categories.Where(c => !c.IsDeleted);
            }
        }
        protected CustomCategoryStore Store { get; private set; }
        protected bool disposed = false;

        #endregion // </FiledsAndProperties>

        #region // <Constructors>

        public CustomCategoryManager(CustomCategoryStore categoryStore)
        {
            this.Store = categoryStore;
        }

        #endregion // </Constructors>

        public static CustomCategoryManager Create(IdentityFactoryOptions<CustomCategoryManager> options, IOwinContext context)
        {
            return new CustomCategoryManager(new CustomCategoryStore(context.Get<CustomDbContext>()));
        }

        public virtual async Task<IdentityResult> CreateCustomAsync(string name, string description = null, int? code = null)
        {
            return await Store.CreateAsync(name, description, code);
        }
        public virtual async Task<IdentityResult> DeleteAsync(string name)
        {
            return await Store.DeleteAsync(name);
        }
        public virtual async Task<CustomCategory> FindByIdAsync(int idCategory)
        {
            return await Store.FindByIdAsync(idCategory);
        }
        public virtual async Task<CustomCategory> FindByNameAsync(string categoryName)
        {
            return await Store.FindByNameAsync(categoryName);
        }
        public virtual async Task<bool> CategoryExistsAsync(string categoryName)
        {
            return await Store.CategoryExistsAsync(categoryName);
        }
        public virtual async Task<IdentityResult> UpdateAsync(int id, string name, string description = null, int? code = null)
        {
            return await Store.UpdateAsync(id, name, description, code);
        }
        public async Task<IList<string>> GetCategoriesAsync(int userId)
        {
            return await Categories
                .Where(c => c.CategoryUsers.Where(u => u.UserId == userId).Any())
                .Select(c => c.Name)
                .ToListAsync();
        }

        #region // <Dispose>

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                if (Store != null)
                {
                    Store.Dispose();
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
    }
}
