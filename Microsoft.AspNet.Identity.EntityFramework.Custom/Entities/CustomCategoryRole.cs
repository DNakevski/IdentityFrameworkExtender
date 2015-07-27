using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    [Table("AspNetCategoryRoles")]
    public partial class CustomCategoryRole
    {
        public CustomCategoryRole()
        {
            this.CreatedOn = DateTime.Now;
        }

        [Key, ForeignKey("Category"), Column(Order = 0)]
        public int CategoryId { get; set; }

        [Key, ForeignKey("Role"), Column(Order = 1)]
        public int RoleId { get; set; }

        [Column("CreatedOn", TypeName = "smalldatetime", Order = 2)]
        public DateTime CreatedOn { get; set; }

        public virtual CustomCategory Category { get; set; }
        public virtual CustomRole Role { get; set; }
    }
}
