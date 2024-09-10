using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace SMTTrainer
{
    public class EmployeesManager
    {
        private readonly ConfigEntry<int> _maxEmployeesConfig;
        private readonly ConfigEntry<float> _employeeSpeedConfig;
        private readonly ManualLogSource _logger;
        private Rect _windowRect = new Rect(0, 0, 300, 150);
        private bool _showWindow;

        public EmployeesManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;
            _maxEmployeesConfig = config.Bind<int>(
                "员工设置",
                "最大员工数量",
                3,
                new ConfigDescription("可支配员工最大数量", null, new ConfigurationManagerAttributes { Browsable = false }));

            _employeeSpeedConfig = config.Bind<float>(
                "员工设置",
                "员工工作速度",
                0.0f,
                new ConfigDescription("员工工作速度（移动速度？", null, new ConfigurationManagerAttributes { Browsable = false }));

            ApplySettings();
        }

        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(2, _windowRect, DrawWindowContent, "员工修改器");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            GUILayout.Label($"最大员工数量： {_maxEmployeesConfig.Value}");
            GUILayout.Label($"员工工作速度： {_employeeSpeedConfig.Value}");

            GUILayout.Label("设置最大员工数量：");
            _maxEmployeesConfig.Value = (int)GUILayout.HorizontalSlider(_maxEmployeesConfig.Value, 1, 100);

            GUILayout.Label("设置员工工作速度：");
            _employeeSpeedConfig.Value = GUILayout.HorizontalSlider(_employeeSpeedConfig.Value, 0.1f, 5.0f);

            if (GUILayout.Button("应用设置"))
            {
                ApplySettings();
            }

            GUI.DragWindow();
        }

        private void ApplySettings()
        {
            if (NPC_Manager.Instance != null)
            {
                NPC_Manager.Instance.maxEmployees = _maxEmployeesConfig.Value;
                NPC_Manager.Instance.extraEmployeeSpeedFactor = _employeeSpeedConfig.Value;
                NPC_Manager.Instance.UpdateEmployeesNumberInBlackboard();
                _logger.LogInfo("员工设置已经应用");
            }
            else
            {
                _logger.LogWarning("NPC_Manager.Instance未加载，未成功设置员工属性");
            }
        }
    }
}
