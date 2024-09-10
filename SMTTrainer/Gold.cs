using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace SMTTrainer
{
    public class GoldManager
    {
        private readonly ConfigEntry<int> _goldAmountConfig;
        private readonly ManualLogSource _logger;
        private Rect _windowRect = new Rect(0, 0, 300, 150);
        private bool _showWindow;
        private string _tempGoldAmount;

        public GoldManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;
            _goldAmountConfig = config.Bind<int>(
                "Gold",
                "Add Gold Amount",
                1000,
                new ConfigDescription("The amount of gold to add in the game", null, new ConfigurationManagerAttributes { Browsable = false }));

            _tempGoldAmount = _goldAmountConfig.Value.ToString();
        }

        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(0, _windowRect, DrawWindowContent, "Gold Trainer");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            GUILayout.Label($"Last added Gold: {_goldAmountConfig.Value}");

            GUILayout.Label("Add Gold Amount:");
            _tempGoldAmount = GUILayout.TextField(_tempGoldAmount);

            if (GUILayout.Button("Confirm"))
            {
                if (int.TryParse(_tempGoldAmount, out int newGoldAmount))
                {
                    SetGoldAmount(newGoldAmount);
                    _goldAmountConfig.Value = newGoldAmount;
                }
                else
                {
                    _logger.LogError("Invalid input! Please enter a valid integer for the gold amount.");
                }
            }

            GUI.DragWindow();
        }

        private void SetGoldAmount(int newAmount)
        {
            if (GameData.Instance != null)
            {
                GameData.Instance.CmdAlterFundsWithoutExperience(newAmount);
                _logger.LogInfo($"Gold in game set to: {newAmount}");
            }
            else
            {
                _logger.LogWarning("GameData.Instance is null, can't set gold.");
            }
        }
    }
}
