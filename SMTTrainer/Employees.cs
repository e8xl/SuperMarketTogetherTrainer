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

        private bool _settingsApplied = false;

        public EmployeesManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;
            _maxEmployeesConfig = config.Bind<int>(
                "Employee Settings",
                "Max Employees",
                1,
                new ConfigDescription("Maximum number of employees", null, new ConfigurationManagerAttributes { Browsable = false }));

            _employeeSpeedConfig = config.Bind<float>(
                "Employee Settings",
                "Employee Speed Factor",
                0.0f,
                new ConfigDescription("Speed factor for employees", null, new ConfigurationManagerAttributes { Browsable = false }));

        }

        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(2, _windowRect, DrawWindowContent, "Employees Trainer");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            GUILayout.Label($"Max Employees: {_maxEmployeesConfig.Value}");
            GUILayout.Label($"Employee Speed Factor: {_employeeSpeedConfig.Value}");

            GUILayout.Label("Set Max Employees:");
            _maxEmployeesConfig.Value = (int)GUILayout.HorizontalSlider(_maxEmployeesConfig.Value, 1, 100);

            GUILayout.Label("Set Employee Speed Factor:");
            _employeeSpeedConfig.Value = GUILayout.HorizontalSlider(_employeeSpeedConfig.Value, 0.1f, 5.0f);

            if (GUILayout.Button("Apply Settings"))
            {
                ApplySettings();
            }

            GUI.DragWindow();
        }
        public void Update()
        {
            if (!_settingsApplied && NPC_Manager.Instance != null)
            {
                ApplySettings();
                _settingsApplied = true; // 确保只应用一次
            }
        }
        private void ApplySettings()
        {
            if (NPC_Manager.Instance != null)
            {
                NPC_Manager.Instance.maxEmployees = _maxEmployeesConfig.Value;
                NPC_Manager.Instance.extraEmployeeSpeedFactor = _employeeSpeedConfig.Value;
                NPC_Manager.Instance.UpdateEmployeesNumberInBlackboard();
                _logger.LogInfo("Employee settings applied.");
            }
            else
            {
                _logger.LogWarning("NPC_Manager.Instance is null, can't apply employee settings.");
            }
        }
    }
}
