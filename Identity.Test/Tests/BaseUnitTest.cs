using Microsoft.AspNet.Identity.EntityFramework;

namespace Itentity.Test
{
    public class BaseUnitTest
    {
       protected IdentityManager man;
       protected IdentityManager Manager { get { if (man == null) { man = new IdentityManager(); } return man; } }
    }
}
