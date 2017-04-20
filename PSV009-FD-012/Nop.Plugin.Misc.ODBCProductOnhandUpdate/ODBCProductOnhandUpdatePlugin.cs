using Nop.Core.Data;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Services.Localization;
using System.Linq;


namespace Nop.Plugin.Misc.ODBCProductOnhandUpdate
{
    public class ODBCProductOnhandUpdate : BasePlugin
    {
        private IRepository<ScheduleTask> _scheduleTaskService;
        private IPluginFinder _pluginFinder;

        public ODBCProductOnhandUpdate(IRepository<ScheduleTask> scheduleTaskService,
                                            IPluginFinder pluginFinder)             
        {
            this._scheduleTaskService = scheduleTaskService;
            this._pluginFinder = pluginFinder;
        }

        public void InstallScheduleTask()
        {
            ScheduleTask data = new ScheduleTask { Enabled = true,
                                                   Name = "ODBC Product onhand updating",
                                                   Seconds = 3600,
                                                   StopOnError = false,
                                                   Type = "Nop.Plugin.Misc.ODBCProductOnhandUpdate.ODBCProductOnhandUpdateServiceTask, Nop.Plugin.Misc.ODBCProductOnhandUpdate"
            };

            _scheduleTaskService.Insert(data);
        }

        public void UninstallScheduleTask()
        {
            ScheduleTask task = _scheduleTaskService.Table.Where(p => p.Type == "Nop.Plugin.Misc.ODBCProductOnhandUpdate.ODBCProductPriceOnhandServiceTask, Nop.Plugin.Misc.ODBCProductOnhandUpdate").FirstOrDefault();

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
                this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCProductOnhandUpdate.Taskname", "Task name");
                this.AddOrUpdatePluginLocaleResource("Plugins.Misc.ODBCProductOnhandUpdate.Taskname.Hint", "Task Name");

                base.Install();
            }
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCProductOnhandUpdate.Taskname");
            this.DeletePluginLocaleResource("Plugins.Misc.ODBCProductOnhandUpdate.Taskname.Hint");

            UninstallScheduleTask();

            base.Uninstall();
        }
     }
}
                                                            