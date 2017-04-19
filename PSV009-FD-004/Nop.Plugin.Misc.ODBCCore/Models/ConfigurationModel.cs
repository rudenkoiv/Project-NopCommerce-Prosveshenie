using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.ODBCCore.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.Misc.ODBCCore.ODBCName")]
        public string ODBCName { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ODBCCore.Username")]
        public string Username { get; set; }

        [NopResourceDisplayName("Plugins.Misc.ODBCCore.Password")]
        public string Password { get; set; }
    }
}
