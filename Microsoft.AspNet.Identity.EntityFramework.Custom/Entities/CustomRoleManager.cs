using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomRoleManager : RoleManager<CustomRole, int>
    {
        /// <summary>
        /// public CustomRoleManager(IRoleStore<CustomRole, int> roleStore)
        /// </summary>
        /// <param name="roleStore"></param>
        public CustomRoleManager(IRoleStore<CustomRole, int> roleStore)
            : base(roleStore)
        {

        }

        public static CustomRoleManager Create(IdentityFactoryOptions<CustomRoleManager> options, IOwinContext context)
        {
            return new CustomRoleManager(new CustomRoleStore(context.Get<CustomDbContext>()));
        }

        public virtual bool IsInCategory(int roleId, string categoryName)
        {
            return (Store as CustomRoleStore).IsInCategory(roleId, categoryName);
        }

        public virtual async Task<IdentityResult> CreateCustomAsync(string name)
        {
            return await base.CreateAsync(new CustomRole { Name = name });
        }

        public virtual async Task<IdentityResult> AddToCategoryAsync(int roleId, string name)
        {
            return await (Store as CustomRoleStore).AddToCategoryAsync(roleId, name);
        }

        public virtual async Task<IdentityResult> RemoveFromCategoryAsync(int roleId, string name)
        {
            return await (Store as CustomRoleStore).RemoveFromCategoryAsync(roleId, name);
        }

        public virtual async Task<IdentityResult> CreateAsync(string name)
        {
            return await base.CreateAsync(new CustomRole { Name = name });
        }

        public virtual async Task<IdentityResult> AddToCategoryAndRemoveTheOthersAsync(int[] roleIds, string name)
        {
            return await (Store as CustomRoleStore).AddToCategoryAndRemoveTheOthersAsync(roleIds, name);
        }

        //public async Task<IdentityResult> CreateRoles(IEnumerable<string> roles)
        public async Task<IdentityResult> CreateRolesFromEnum<T>() where T : struct, IConvertible, IComparable, IFormattable
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new ArgumentException("T is not Enumerable");
            }

            foreach (T role in Enum.GetValues(type).Cast<T>())
            {
                var result = await CreateAsync(role.ToString());
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            return IdentityResult.Success;
        }
    }
}
