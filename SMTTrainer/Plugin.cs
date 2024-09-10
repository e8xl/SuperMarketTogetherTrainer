using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;

namespace SMTTrainer
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Supermarket Together.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<bool> _isGoldWindowEnabled;
        private ConfigEntry<bool> _isPointWindowEnabled;
        private ConfigEntry<bool> _isEmployeesWindowEnabled;

        private GoldManager _goldManager;
        private PointManager _pointManager;
        private EmployeesManager _employeesManager;

        private void Awake()
        {
            Logger.LogInfo($"插件 {MyPluginInfo.PLUGIN_GUID} 加载成功");

            // 注册goldManager,pointManager,employeesManager
            _goldManager = new GoldManager(Config, Logger);
            _pointManager = new PointManager(Config, Logger);
            _employeesManager = new EmployeesManager(Config, Logger);

            // 绑定配置以显示Gold窗口
            _isGoldWindowEnabled = Config.Bind<bool>(
                "修改器窗口",
                "启用金币修改器",
                false,
                "打开金币修改器");

            // 绑定配置以显示Point窗口
            _isPointWindowEnabled = Config.Bind<bool>(
                "修改器窗口",
                "启用特许经营点数修改器",
                false,
                "打开金币修改器");

            // 绑定配置以显示Employees窗口
            _isEmployeesWindowEnabled = Config.Bind<bool>(
                "修改器窗口",
                "启用员工修改器",
                false,
                "打开员工修改器窗口");

            // 设置配置更改事件
            _isGoldWindowEnabled.SettingChanged += OnGoldWindowEnableChanged;
            _isPointWindowEnabled.SettingChanged += OnPointWindowEnableChanged;
            _isEmployeesWindowEnabled.SettingChanged += OnEmployeesWindowEnableChanged;
        }

        private void OnGoldWindowEnableChanged(object sender, System.EventArgs e)
        {
            _goldManager.SetWindowVisibility(_isGoldWindowEnabled.Value);
        }

        private void OnPointWindowEnableChanged(object sender, System.EventArgs e)
        {
            _pointManager.SetWindowVisibility(_isPointWindowEnabled.Value);
        }

        private void OnEmployeesWindowEnableChanged(object sender, System.EventArgs e)
        {
            _employeesManager.SetWindowVisibility(_isEmployeesWindowEnabled.Value);
        }

        private void OnGUI()
        {
            // 绘制Gold窗口
            if (_isGoldWindowEnabled.Value)
            {
                _goldManager.DrawWindow();
            }

            // 绘制Point窗口
            if (_isPointWindowEnabled.Value)
            {
                _pointManager.DrawWindow();
            }

            // 绘制Employees窗口
            if (_isEmployeesWindowEnabled.Value)
            {
                _employeesManager.DrawWindow();
            }
        }
    }
}
