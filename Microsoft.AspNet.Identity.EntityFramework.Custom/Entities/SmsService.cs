using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
