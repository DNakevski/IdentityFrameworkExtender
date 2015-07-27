using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class IdentityManager : IDisposable
    {
        protected bool disposed = false;

        protected CustomDbContext context;
        protected CustomDbContext Context { get { if (context == null) { context = new CustomDbContext(); } return context; } }

        protected CustomCategoryStore categoryStore;
        protected CustomCategoryStore CategoryStore { get { if (categoryStore == null) { categoryStore = new CustomCategoryStore(Context); } return categoryStore; } }

        protected CustomRoleStore roleStore;
        protected CustomRoleStore RoleStore { get { if (roleStore == null) { roleStore = new CustomRoleStore(Context); } return roleStore; } }

        protected CustomUserStore userStore;
        protected CustomUserStore UserStore { get { if (userStore == null) { userStore = new CustomUserStore(Context); } return userStore; } }
        
        protected CustomCategoryManager categoryManager;
        public CustomCategoryManager CategoryManager { get { if (categoryManager == null) { categoryManager = new CustomCategoryManager(CategoryStore); } return categoryManager; } }

        protected CustomUserManager userManager;
        public CustomUserManager UserManager { get { if (userManager == null) { userManager = new CustomUserManager(UserStore); } return userManager; } }

        protected CustomRoleManager roleManager;
        public CustomRoleManager RoleManager { get { if (roleManager == null) { roleManager = new CustomRoleManager(RoleStore); } return roleManager; } }

        public IdentityManager()
        {

        }

        public IdentityManager(CustomDbContext ctx)
        {
            context = ctx;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Cleanup
                if (userManager != null)
                {
                    userManager.Dispose();
                }
                
                if (roleManager != null)
                {
                    roleManager.Dispose();
                }

                if (categoryManager != null)
                {
                    categoryManager.Dispose();
                }

                if (userStore != null)
                {
                    userStore.Dispose();
                }

                if (roleStore != null)
                {
                    roleStore.Dispose();
                }
                
                if (categoryStore != null)
                {
                    categoryStore.Dispose();
                }
                
                if (context != null)
                {
                    context.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
