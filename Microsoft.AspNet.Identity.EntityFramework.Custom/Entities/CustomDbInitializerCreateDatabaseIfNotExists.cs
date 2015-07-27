using System;
using System.Data.Entity;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public class CustomDbInitializerCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<CustomDbContext>
    {
        Action<CustomDbContext> Seeder;

        public CustomDbInitializerCreateDatabaseIfNotExists(Action<CustomDbContext> seeder)
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
