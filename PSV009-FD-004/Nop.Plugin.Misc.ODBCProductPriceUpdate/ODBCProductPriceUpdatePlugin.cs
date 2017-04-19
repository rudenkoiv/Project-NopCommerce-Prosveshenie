using Nop.Core.Data;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Services.Localization;
using System.Linq;


namespace Nop.Plugin.Misc.ODBCProductPriceUpdate
{
    public class ODBCProductPriceUpdatePlugin : BasePlugin
    {
        private IRepository<ScheduleTask> _scheduleTaskService;
        private IPluginFinder _pluginFinder;

        public ODBCProductPriceUpdatePlugin(IRepository<ScheduleTask> scheduleTaskService,
                                            IPluginFinder pluginFinder)             
        {
            this._scheduleTaskService = scheduleTaskService;
            this._pluginFinder = pluginFinder;
        }

        public void InstallScheduleTask()
        {
            ScheduleTask data = new ScheduleTask { Enabled = true,
                                                   Name = "ODBC Product price updating",
                                                   Seconds = 3600,
                                                   StopOnError = false,
                                                   Type = "Nop.Plugin.Misc.ODBCProductPriceUpdate.ODBCProductPriceUpdateServiceTask, Nop.Plugin.Misc.ODBCProductPriceUpdate"
            };

            _scheduleTaskService.Insert(data);
        }

        public void UninstallScheduleTask()
        {
            ScheduleTask task = _scheduleTaskService.Table.Where(p => p.Type == "Nop.Plugin.Misc.ODBCProductPriceUpdate.ODBCProductPriceUpdateServiceTask, Nop.Plugin.Misc.ODBCProductPriceUpdate").FirstOrDefault();

            if (task != null)
            {
                _scheduleTaskService.Delete(task);
            }

        }


        public override void Install()
        {
            PluginDescriptor corePlugin = _pluginFinder.GetPluginDescriptorBySystemName("Misc.ODBCCore");

            if (corePlugin != null && corePlugin.Installed)
            {
                InstallScheduleTask();

                //locales
                this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCProductPriceUpdate.Taskname", "Task name");
                this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCProductPriceUpdate.Taskname.Hint", "Task Name");

                base.Install();
            }
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCProductPriceUpdate.Taskname");
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCProductPriceUpdate.Taskname.Hint");

            UninstallScheduleTask();

            base.Uninstall();
        }
     }
}
                                                            