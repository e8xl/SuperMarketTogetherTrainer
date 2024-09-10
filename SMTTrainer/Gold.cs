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
                "金币",
                "添加金币数量",
                1000,
                new ConfigDescription("添加金币到游戏中", null, new ConfigurationManagerAttributes { Browsable = false }));

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
                _windowRect = GUILayout.Window(0, _windowRect, DrawWindowContent, "金币修改器");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            GUILayout.Label($"上次添加的金币数量： {_goldAmountConfig.Value}");

            GUILayout.Label("添加金币数量：");
            _tempGoldAmount = GUILayout.TextField(_tempGoldAmount);

            if (GUILayout.Button("提交"))
            {
                if (int.TryParse(_tempGoldAmount, out int newGoldAmount))
                {
                    SetGoldAmount(newGoldAmount);
                    _goldAmountConfig.Value = newGoldAmount;
                }
                else
                {
                    _logger.LogError("数值类型错误！");
                }
            }

            GUI.DragWindow();
        }

        private void SetGoldAmount(int newAmount)
        {
            if (GameData.Instance != null)
            {
                GameData.Instance.CmdAlterFundsWithoutExperience(newAmount);
                _logger.LogInfo($"金币添加了： {newAmount}");
            }
            else
            {
                _logger.LogWarning("GameData.Instance巴拉巴拉巴拉巴拉");
            }
        }
    }
}
