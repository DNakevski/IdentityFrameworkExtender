using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public partial class CustomRole : IdentityRole<int, CustomUserRole>
    {
        partial void InitializeProperties();

        public CustomRole()
            : this("", null)
        {
        }

        public CustomRole(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// If you need to set default value to fields use InitializeProperties partial method
        /// </summary>
        public CustomRole(string name, Nullable<int> parent)
            : base()
        {
            this.CategoryRoles = new HashSet<CustomCategoryRole>();
            this.Name = name;
            this.CreatedOn = DateTime.Now;
            this.ParentRoleId = parent;
            this.TranslationCode = -1;
            InitializeProperties();
        }

        [Column("CreatedOn", TypeName = "smalldatetime")]
        public DateTime CreatedOn { get; set; }

        [DefaultValue("-1")]
        [Column("TranslationCode")]
        public int TranslationCode { get; set; }

        [Column("ParentRoleId")]
        public Nullable<int> ParentRoleId { get; set; }

        public virtual CustomRole ParentRole { get; set; }
        public virtual ICollection<CustomRole> ChildRoles { get; set; }

        public virtual ICollection<CustomCategoryRole> CategoryRoles { get; set; }
    }
}
