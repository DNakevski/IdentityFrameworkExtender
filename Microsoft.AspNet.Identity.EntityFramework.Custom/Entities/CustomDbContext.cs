using System.Configuration;
using System.Data.Entity;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public partial class CustomDbContext : IdentityDbContext<CustomUser, CustomRole,
        int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomDbContext()
            : base("DefaultConnection")
        {

            if (Config.UseCustomDbInitializer)
            {
                Database.SetInitializer<CustomDbContext>(new DropCreateDatabaseAlways<CustomDbContext>());
                //Database.SetInitializer<CustomDbContext>(new CustomDbInitializerDropCreateDatabaseAlways(db => Seed(db)));
                //var context = new CustomDbContext();
                //context.Database.Initialize(true);
            }
        }

        public static CustomDbContext Create()
        {
            return new CustomDbContext();
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region // <Direct Many To Many CategoryRoles>
            //modelBuilder
            //    .Entity<CustomCategory>()
            //    .HasMany(c => c.Roles)
            //    .WithMany(r => r.Categories)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("CategoryId");
            //        m.MapRightKey("RoleId");
            //        m.ToTable("AspNetCategoryRoles");
            //    });
            #endregion // </Direct Many To Many CategoryRoles>

            #region // <Direct Many To Many CategoryUsers>
            //modelBuilder
            //    .Entity<CustomCategory>()
            //    .HasMany(c => c.Users)
            //    .WithMany(r => r.Categories)
            //    .Map(m =>
            //    {
            //        m.MapLeftKey("CategoryId");
            //        m.MapRightKey("UserId");
            //        m.ToTable("AspNetCategoryUsers");
            //    });
            #endregion // </Direct Many To Many CategoryUsers>

            modelBuilder
                .Entity<CustomRole>()
                .HasOptional(i => i.ParentRole)
                .WithMany(i => i.ChildRoles)
                .HasForeignKey(i => i.ParentRoleId);
        }

        public DbSet<CustomCategory> Categories { get; set; }
        public DbSet<CustomCategoryRole> CategoryRoles { get; set; }
        public DbSet<CustomCategoryUser> CategoryUsers { get; set; }
        public DbSet<CustomUserRole> UserRoles { get; set; }
    }
}
