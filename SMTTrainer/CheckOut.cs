using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace SMTTrainer
{
    public class CheckoutManager
    {
        private readonly ConfigEntry<float> _checkoutMultiplierConfig;
        private readonly ManualLogSource _logger;
        private Rect _windowRect = new Rect(0, 0, 300, 150);
        private bool _showWindow;

        private bool _settingsApplied = false;

        public CheckoutManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;
            _checkoutMultiplierConfig = config.Bind<float>(
                "Checkout Settings",
                "Checkout Multiplier",
                1.0f,
                new ConfigDescription("Multiplier for checkout payments", null, new ConfigurationManagerAttributes { Browsable = false }));
        }

        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(3, _windowRect, DrawWindowContent, "Checkout Trainer");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            GUILayout.Label($"Checkout Multiplier: {_checkoutMultiplierConfig.Value}");

            GUILayout.Label("Set Checkout Multiplier:");
            _checkoutMultiplierConfig.Value = GUILayout.HorizontalSlider(_checkoutMultiplierConfig.Value, 0.5f, 5.0f);

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
                NPC_Manager.Instance.extraCheckoutMoney = _checkoutMultiplierConfig.Value;
                _logger.LogInfo("Checkout settings applied.");
            }
            else
            {
                _logger.LogWarning("NPC_Manager.Instance is null, can't apply checkout settings.");
            }
        }
    }
}
