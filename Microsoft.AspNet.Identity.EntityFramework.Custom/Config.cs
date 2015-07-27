using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Entity;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    static internal class Config
    {
        internal static T Get<T>(string application, T defaultValue)
        {
            string appSetting = ConfigurationManager.AppSettings[application];

            if (string.IsNullOrEmpty(appSetting))
            {
                return defaultValue;
            }

            return (T)Convert.ChangeType(appSetting, typeof(T));
        }

        public static bool UseCustomDbInitializer
        {
            get
            {
                return Get<bool>("UseCustomDbInitializer", false);
            }
        }
    }
}
