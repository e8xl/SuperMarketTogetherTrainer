using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace SMTTrainer
{
    public class PointManager
    {
        private readonly ConfigEntry<int> _pointAmountConfig;
        private readonly ManualLogSource _logger;
        private Rect _windowRect = new Rect(0, 0, 300, 150);
        private bool _showWindow;
        private string _tempPointAmount;

        public PointManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;
            _pointAmountConfig = config.Bind<int>(
                "Point",
                "Set Point Amount",
                1,
                new ConfigDescription("The amount of points to set in the game", null, new ConfigurationManagerAttributes { Browsable = false }));

            _tempPointAmount = _pointAmountConfig.Value.ToString();
        }

        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(1, _windowRect, DrawWindowContent, "Point Trainer");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            GUILayout.Label($"Current Points: {_pointAmountConfig.Value}");

            GUILayout.Label("Set New Point Amount:");
            _tempPointAmount = GUILayout.TextField(_tempPointAmount);

            if (GUILayout.Button("Confirm"))
            {
                if (int.TryParse(_tempPointAmount, out int newPointAmount))
                {
                    SetPointAmount(newPointAmount);
                    _pointAmountConfig.Value = newPointAmount;
                }
                else
                {
                    _logger.LogError("Invalid input! Please enter a valid integer for the point amount.");
                }
            }

            GUI.DragWindow();
        }

        private void SetPointAmount(int newAmount)
        {
            if (GameData.Instance != null)
            {
                GameData.Instance.NetworkgameFranchisePoints = newAmount;
                _logger.LogInfo($"Points in game set to: {newAmount}");
            }
            else
            {
                _logger.LogWarning("GameData.Instance is null, can't set points.");
            }
        }
    }
}
