using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ODBCCore
{
    public class ODBCCorePlugin : BasePlugin, IMiscPlugin
    {
        public ODBCCoreSettings _odbcSettings;
        public ISettingService _settingService;
        public ODBCCorePlugin(
        ISettingService settingService,
        ODBCCoreSettings odbcSettings)             
        {
            this._settingService = settingService;
            this._odbcSettings = odbcSettings;
        }
        
        public override void Install()
        {
            var settings = new ODBCCoreSettings()
            {
                ODBCName = "",
                Username = "",
                Password = "" 
            };

            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCCore.ODBCName", "ODBC connection Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCCore.ODBCName.Hint", "Insert ODBC connection name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCCore.Username", "Username for ODBC connection");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCCore.Username.Hint", "Insert username for ODBC connection");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCCore.Password", "Password for ODBC connection");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCCore.Password.Hint", "Insert password for ODBC connection");

            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCCore.ODBCName");
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCCore.ODBCName.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCCore.Username");
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCCore.Username.Hint");
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCCore.Password");
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCCore.Password.Hint");

            base.Uninstall();
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ODBCCore";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Misc.ODBCCore.Controllers" }, { "area", null } };
        }
    }
}
                                                            