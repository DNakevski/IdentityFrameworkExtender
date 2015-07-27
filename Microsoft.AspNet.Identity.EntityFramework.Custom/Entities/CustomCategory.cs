using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    [Table("AspNetCategories")]
    public partial class CustomCategory
    {
        partial void InitializeProperties();

        /// <summary>
        /// If you need to set default value to fields use InitializeProperties partial method
        /// </summary>
        public CustomCategory()
        {
            this.CategoryRoles = new HashSet<CustomCategoryRole>();
            this.CategoryUsers = new HashSet<CustomCategoryUser>();
            this.DeletedOn = DefaultValues.SmallDateTimeMax;
            this.Name = "";
            this.TranslationCode = -1;
            this.Description = "";
            this.CreatedOn = DateTime.Now;
            InitializeProperties();
        }

        [Key]
        public int Id { get; set; }

        [Column("CreatedOn", TypeName = "smalldatetime")]
        public DateTime CreatedOn { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        [Column("DeletedOn", TypeName = "smalldatetime")]
        public DateTime DeletedOn { get; set; }

        [DefaultValue("-1")]
        public int TranslationCode { get; set; }

        [DefaultValue("")]
        public string Name { get; set; }

        [DefaultValue("")]
        public string Description { get; set; }

        public virtual ICollection<CustomCategoryRole> CategoryRoles { get; set; }
        public virtual ICollection<CustomCategoryUser> CategoryUsers { get; set; }
    }
}
