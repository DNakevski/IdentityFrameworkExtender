using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class AccountWorkerService : IDisposable
    {
        #region // <Preperties>
        protected bool disposed = false;

        private IOwinContext _owinContext;
        protected IOwinContext OwinContext
        {
            get { return _owinContext; }
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

        private IAuthenticationManager _authenticationManager;
        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return _authenticationManager ?? OwinContext.Authentication;
            }
            private set { _authenticationManager = value; }
        }

        #endregion // </Preperties>

        public AccountWorkerService()
        {

        }

        public AccountWorkerService(IOwinContext owinContext)
        {
            this.OwinContext = owinContext;
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
                if (_userManager != null)
                {
                    _userManager.Dispose();
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<SignInStatus> SignIn(string userName, string password, bool isPersistent, bool shouldLockout)
        {            
            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            return await SignInManager.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
        }
    }
}
