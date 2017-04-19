using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ODBCCore
{
    public class ODBCCoreSettings : ISettings
    {
        public string ODBCName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
