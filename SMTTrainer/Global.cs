using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace SMTTrainer
{
    public class GlobalManager
    {
        private readonly ConfigEntry<float> _timeConfig;
        public ConfigEntry<bool> _lockTimeConfig;
        private readonly ManualLogSource _logger;
        private Rect _windowRect = new Rect(0, 0, 300, 150);
        private bool _showWindow;
        private string _tempTimeInput = "12:00";  // 默认时间输入
        private float _lockedTime = -1f;  // 存储锁定时的时间

        public GlobalManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;

            _timeConfig = config.Bind<float>(
                "Global",
                "Time",
                12f,
                new ConfigDescription("Current game time in hours, e.g., 12.5 = 12:30", null, new ConfigurationManagerAttributes { Browsable = false }));

            _lockTimeConfig = config.Bind<bool>(
                "Global",
                "Lock Time",
                false,
                new ConfigDescription("Lock the time at the current value", null, new ConfigurationManagerAttributes { Browsable = false }));
        }

        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(5, _windowRect, DrawWindowContent, "Global Time Trainer");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            // 当前游戏时间显示
            GUILayout.Label($"Current Time: {ConvertTimeToDisplay(GameData.Instance?.NetworktimeOfDay ?? _timeConfig.Value)}");

            // 输入框以小时和分钟的形式接受用户输入
            GUILayout.Label("Set Time (format HH:MM):");
            _tempTimeInput = GUILayout.TextField(_tempTimeInput);

            // 确认按钮
            if (GUILayout.Button("Confirm"))
            {
                if (TryParseTimeInput(_tempTimeInput, out float parsedTime))
                {
                    SetGameTime(parsedTime);
                    _logger.LogInfo($"Time set to: {_tempTimeInput} (float value: {parsedTime})");
                }
                else
                {
                    _logger.LogError("Invalid time input. Please use HH:MM format.");
                }
            }
            bool lockTime = GUILayout.Toggle(_lockTimeConfig.Value, "Lock Time");
            GUILayout.Label("The store can only be opened and closed if the time is correct.");
            GUILayout.Label("Open time:8:00; Closing time:22:30");

            // 锁定时间的复选框
            
            if (lockTime != _lockTimeConfig.Value)
            {
                _lockTimeConfig.Value = lockTime;

                if (lockTime)
                {
                    // 如果锁定时间，保存当前的游戏时间
                    _lockedTime = GameData.Instance.NetworktimeOfDay;
                    _logger.LogInfo($"Time locked at: {ConvertTimeToDisplay(_lockedTime)}");
                }
                else
                {
                    _lockedTime = -1f;  // 解锁时重置锁定时间
                    _logger.LogInfo("Time unlocked.");
                }
            }

            GUI.DragWindow();
        }

        private void SetGameTime(float newTime)
        {
            // 修改游戏内时间为用户输入的时间
            if (GameData.Instance != null)
            {
                _timeConfig.Value = newTime; // 更新 Config 中的值
                GameData.Instance.NetworktimeOfDay = newTime;  // 立即修改游戏时间
            }
            else
            {
                _logger.LogWarning("GameData.Instance is null, can't set game time.");
            }
        }

        // 将时间转换为 HH:MM 格式的字符串用于显示
        private string ConvertTimeToDisplay(float timeValue)
        {
            int hours = Mathf.FloorToInt(timeValue);
            int minutes = Mathf.FloorToInt((timeValue - hours) * 60);
            return $"{hours:D2}:{minutes:D2}";
        }

        // 解析用户输入的时间字符串（HH:MM）为 float 类型
        private bool TryParseTimeInput(string timeInput, out float parsedTime)
        {
            parsedTime = 0f;
            string[] parts = timeInput.Split(':');
            if (parts.Length != 2) return false;

            if (int.TryParse(parts[0], out int hours) && int.TryParse(parts[1], out int minutes))
            {
                if (hours >= 0 && hours < 24 && minutes >= 0 && minutes < 60)
                {
                    parsedTime = hours + minutes / 60f;
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            // 当锁定时间时，保持游戏时间为锁定时的时间
            if (_lockTimeConfig.Value && GameData.Instance != null && _lockedTime >= 0f)
            {
                GameData.Instance.NetworktimeOfDay = _lockedTime;
            }
        }
    }
}
