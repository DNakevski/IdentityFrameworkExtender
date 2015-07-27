using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomUserManager : UserManager<CustomUser, int>
    {
        public CustomUserManager(IUserStore<CustomUser, int> store)
            : base(store)
        {
        }

        public static CustomUserManager Create(IdentityFactoryOptions<CustomUserManager> options,
            IOwinContext context)
        {
            var manager = new CustomUserManager(new CustomUserStore(context.Get<CustomDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<CustomUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<CustomUser, int>
            {
                MessageFormat = "Your security code is: {0}"
            });

            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<CustomUser, int>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });

            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<CustomUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public CustomUser FindByName(string userName)
        {
            return base.FindByNameAsync(userName).Result;
        }

        public virtual bool IsInCategory(CustomUser user, string categoryName)
        {
            return (Store as CustomUserStore).IsInCategory(user, categoryName);
        }

        public async Task<IdentityResult> CreateCustomAsync(string username, string email, string password)
        {
            return await CreateAsync(new CustomUser() { UserName = username, Email = email }, password);
        }

        public virtual async Task<IdentityResult> AddToCategoryAsync(int userId, string name)
        {
            return await (Store as CustomUserStore).AddToCategoryAsync(userId, name);
        }

        public virtual async Task<IdentityResult> RemoveFromCategoryAsync(int userId, string name)
        {
            return await (Store as CustomUserStore).RemoveFromCategoryAsync(userId, name);
        }
    }
}
