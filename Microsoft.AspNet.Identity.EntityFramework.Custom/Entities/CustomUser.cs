using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public partial class CustomUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        partial void InitializeProperties();

        /// <summary>
        /// If you need to set default value to fields use InitializeProperties partial method
        /// </summary>
        public CustomUser()
        {
            this.CategoryUsers = new HashSet<CustomCategoryUser>();
            this.CreatedOn = DateTime.Now;
            this.DeletedOn = DefaultValues.SmallDateTimeMax;
            InitializeProperties();
        }

        [Column("CreatedOn", TypeName = "smalldatetime")]
        public DateTime CreatedOn { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        [Column("DeletedOn", TypeName = "smalldatetime")]
        public DateTime DeletedOn { get; set; }

        public virtual ICollection<CustomCategoryUser> CategoryUsers { get; set; }
    }
}
