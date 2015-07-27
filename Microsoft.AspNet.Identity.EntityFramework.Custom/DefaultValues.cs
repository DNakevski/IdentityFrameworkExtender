using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity.EntityFramework
{
    public static class DefaultValues
    {
        public static DateTime SmallDateTimeMin { get { return new DateTime(1900, 1, 1); } }
        public static DateTime SmallDateTimeMax { get { return new DateTime(2079, 1, 1); } }
    }
}
