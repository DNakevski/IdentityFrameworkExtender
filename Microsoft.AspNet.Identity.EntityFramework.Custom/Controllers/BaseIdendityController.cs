using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;


namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class BaseIdendityController : Controller
    {
        #region // <Preperties>

        private IOwinContext _owinContext;
        public IOwinContext OwinContext
        {
            get
            {
                return _owinContext ?? HttpContext.GetOwinContext();
            }
            private set { _owinContext = value; }
        }

        private CustomUserManager _userManager;
        public CustomUserManager UserManager
        {
            get
            {
                return _userManager ?? OwinContext.GetUserManager<CustomUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private CustomSignInManager _signInManager;
        public CustomSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? OwinContext.Get<CustomSignInManager>();
            }
            private set { _signInManager = value; }
        }

        private CustomRoleManager _roleManager;
        public CustomRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? OwinContext.Get<CustomRoleManager>();
            }
            private set { _roleManager = value; }
        }

        private CustomCategoryManager _categoryManager;
        public CustomCategoryManager CategoryManager
        {
            get
            {
                return _categoryManager ?? OwinContext.Get<CustomCategoryManager>();
            }
            private set { _categoryManager = value; }
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return OwinContext.Authentication;
            }
        }

        #endregion // </Preperties>

        public BaseIdendityController()
        {
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        protected const string XsrfKey = "XsrfId";

        protected async Task SignInAsync(CustomUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(
                DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.TwoFactorCookie
            );

            AuthenticationManager.SignIn(
                new AuthenticationProperties { IsPersistent = isPersistent },
                UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie).Result
           );
        }

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        protected bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId<int>());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }


        public class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;

            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (_userManager != null)
            {
                _userManager.Dispose();
            }

            if (_signInManager != null)
            {
                _signInManager.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}