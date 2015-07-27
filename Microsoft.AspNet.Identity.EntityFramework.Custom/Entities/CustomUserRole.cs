using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomUserRole : IdentityUserRole<int>
    {
        public CustomUserRole()
            : base()
        {
            this.CreatedOn = DateTime.Now;
        }

        [Column("CreatedOn", TypeName = "smalldatetime")]
        public DateTime CreatedOn { get; set; }
    }
}
