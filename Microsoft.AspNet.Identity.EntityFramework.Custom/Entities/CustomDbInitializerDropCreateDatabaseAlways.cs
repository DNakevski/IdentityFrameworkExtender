using System;
using System.Data.Entity;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomDbInitializerDropCreateDatabaseAlways : DropCreateDatabaseAlways<CustomDbContext>
    {
        Action<CustomDbContext> Seeder;

        public CustomDbInitializerDropCreateDatabaseAlways(Action<CustomDbContext> seeder)
        {
            Seeder = seeder;
        }

        protected override void Seed(CustomDbContext context)
        {
            Seeder(context);
            base.Seed(context);
        }
    }
}
