using System.Web.Security;
using System.Security.Principal;
using System.Web.Mvc;
using System;
using CustomDbContext = Microsoft.AspNet.Identity.EntityFramework.CustomDbContext;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public enum Permissions
    {
        Home,
        Admin
    }
    
    public static class PermissionExtender
    {
        public static UserPermissions HasPermissionOf(this IPrincipal user)
        {
            return new UserPermissions(user);
        }

        public class UserPermissions
        {
            private IPrincipal user;
            public UserPermissions(IPrincipal user)
            {
                this.user = user;
            }

            public bool Admin { get { return user.IsInRole(Permissions.Admin.ToString()); } }
            public bool Home { get { return user.IsInRole(Permissions.Home.ToString()); } }
        }
    }

    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        Permissions Permission;

        public PermissionAuthorizeAttribute(Permissions permission)
            : base()
        {
            Permission = permission;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }

            if (!filterContext.HttpContext.User.IsInRole(Permission.ToString()))
            {
                filterContext.Controller.TempData.Add("RedirectReason", "You are not authorized to access this page.");
                filterContext.Result = new RedirectResult("~/Error/Unauthorized");

                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}