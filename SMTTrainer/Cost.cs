using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace SMTTrainer
{
    public class CostManager
    {
        private readonly ConfigEntry<float> _lightCostMultiplierConfig;
        private readonly ConfigEntry<float> _rentCostMultiplierConfig;
        private readonly ConfigEntry<float> _employeesCostMultiplierConfig;
        private readonly ManualLogSource _logger;
        private Rect _windowRect = new Rect(0, 0, 300, 200);
        private bool _showWindow;

        private bool _settingsApplied = false;

        public CostManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;

            _lightCostMultiplierConfig = config.Bind<float>(
                "Cost Settings",
                "Light Cost Multiplier",
                1.0f,
                new ConfigDescription("Multiplier for light costs", null, new ConfigurationManagerAttributes { Browsable = false }));

            _rentCostMultiplierConfig = config.Bind<float>(
                "Cost Settings",
                "Rent Cost Multiplier",
                1.0f,
                new ConfigDescription("Multiplier for rent costs", null, new ConfigurationManagerAttributes { Browsable = false }));

            _employeesCostMultiplierConfig = config.Bind<float>(
                "Cost Settings",
                "Employees Cost Multiplier",
                1.0f,
                new ConfigDescription("Multiplier for employees costs", null, new ConfigurationManagerAttributes { Browsable = false }));
        }

        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(4, _windowRect, DrawWindowContent, "Cost Trainer");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            GUILayout.Label($"Light Cost Multiplier: {_lightCostMultiplierConfig.Value}");
            _lightCostMultiplierConfig.Value = GUILayout.HorizontalSlider(_lightCostMultiplierConfig.Value, 0.5f, 5.0f);

            GUILayout.Label($"Rent Cost Multiplier: {_rentCostMultiplierConfig.Value}");
            _rentCostMultiplierConfig.Value = GUILayout.HorizontalSlider(_rentCostMultiplierConfig.Value, 0.5f, 5.0f);

            GUILayout.Label($"Employees Cost Multiplier: {_employeesCostMultiplierConfig.Value}");
            _employeesCostMultiplierConfig.Value = GUILayout.HorizontalSlider(_employeesCostMultiplierConfig.Value, 0.5f, 5.0f);

            if (GUILayout.Button("Apply Settings"))
            {
                ApplySettings();
            }

            GUI.DragWindow();
        }

        public void Update()
        {
            if (!_settingsApplied && GameData.Instance != null)
            {
                ApplySettings();
                _settingsApplied = true; // 确保只应用一次
            }
        }

        private void ApplySettings()
        {
            if (GameData.Instance != null)
            {
                UpgradesManager component = GameData.Instance.GetComponent<UpgradesManager>();

                // 应用电费、租金和员工工资倍率
                GameData.Instance.lightCost = (10f + component.spaceBought + component.storageBought) * _lightCostMultiplierConfig.Value;
                GameData.Instance.rentCost = (15f + component.spaceBought * 5 + component.storageBought * 10) * _rentCostMultiplierConfig.Value;
                GameData.Instance.employeesCost = (NPC_Manager.Instance.maxEmployees * 60) * _employeesCostMultiplierConfig.Value;

                _logger.LogInfo("Cost settings applied.");
                _logger.LogInfo($"Applied settings: lightCost={GameData.Instance.lightCost}, rentCost={GameData.Instance.rentCost}, employeesCost={GameData.Instance.employeesCost}");

            }
            else
            {
                _logger.LogWarning("GameData.Instance is null, can't apply cost settings.");
            }
        }
    }
}
