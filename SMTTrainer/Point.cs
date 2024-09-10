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
                "特许经营点数",
                "设置特许经营点数",
                1,
                new ConfigDescription("设置特许经营点数在游戏中", null, new ConfigurationManagerAttributes { Browsable = false }));

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
            GUILayout.Label($"现存点数：{_pointAmountConfig.Value}");

            GUILayout.Label("设置新的特许经营点数：");
            _tempPointAmount = GUILayout.TextField(_tempPointAmount);

            if (GUILayout.Button("提交"))
            {
                if (int.TryParse(_tempPointAmount, out int newPointAmount))
                {
                    SetPointAmount(newPointAmount);
                    _pointAmountConfig.Value = newPointAmount;
                }
                else
                {
                    _logger.LogError("输入数据类型错误！请检查数据类型是否为整数型(int)");
                }
            }

            GUI.DragWindow();
        }

        private void SetPointAmount(int newAmount)
        {
            if (GameData.Instance != null)
            {
                GameData.Instance.NetworkgameFranchisePoints = newAmount;
                _logger.LogInfo($"设置特许经营点数为： {newAmount}");
            }
            else
            {
                _logger.LogWarning("GameData.Instance加载失败，设置特许经营点数失败！");
            }
        }
    }
}
