using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    [Table("AspNetCategoryUsers")]
    public partial class CustomCategoryUser
    {
        public CustomCategoryUser()
        {
            this.CreatedOn = DateTime.Now;
        }

        [Key, ForeignKey("Category"), Column(Order = 0)]
        public int CategoryId { get; set; }

        [Key, ForeignKey("User"), Column(Order = 1)]
        public int UserId { get; set; }

        [Column("CreatedOn", TypeName = "smalldatetime", Order = 2)]
        public DateTime CreatedOn { get; set; }

        public virtual CustomCategory Category { get; set; }
        public virtual CustomUser User { get; set; }
    }
}
