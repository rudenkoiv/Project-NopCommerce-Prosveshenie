using Nop.Plugin.Misc.ODBCCore.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.ODBCCore.Controllers
{
    public class ODBCCoreController : BasePluginController
    {
        private ISettingService _settingService;
        private ODBCCoreSettings _odbcSettings;
        private readonly ILocalizationService _localizationService;

        public ODBCCoreController(ISettingService  settingService,
                                  ODBCCoreSettings odbcSettings,
                                  ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._odbcSettings = odbcSettings;
            this._localizationService = localizationService;
        }
        
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel
            {
                ODBCName = _odbcSettings.ODBCName,
                Username = _odbcSettings.Username,
                Password = _odbcSettings.Password
            };

            return View("~/Plugins/Misc.ODBCCore/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _odbcSettings.ODBCName = model.ODBCName;
            _odbcSettings.Username = model.Username;
            _odbcSettings.Password = model.Password;

            _settingService.SaveSetting(_odbcSettings);

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }
    }
}
