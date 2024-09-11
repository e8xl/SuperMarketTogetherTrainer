using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace SMTTrainer
{
    public class MoveManager
    {
        private readonly ConfigEntry<float> _walkSpeedConfig;
        private readonly ConfigEntry<float> _sprintSpeedConfig;
        private readonly ConfigEntry<float> _airSpeedConfig;
        private readonly ConfigEntry<float> _crouchSpeedConfig;
        private readonly ManualLogSource _logger;

        private Rect _windowRect = new Rect(0, 0, 300, 300);
        private bool _showWindow;

        // 临时变量存储用户输入的速度
        private float _tempWalkSpeed;
        private float _tempSprintSpeed;
        private float _tempAirSpeed;
        private float _tempCrouchSpeed;

        public MoveManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;

            // 配置条目，设置默认值，最小值是当前游戏默认值，添加描述并将其隐藏
            _walkSpeedConfig = config.Bind("Move", "Walk Speed", 5f, new ConfigDescription("Default Walk Speed", null, new ConfigurationManagerAttributes { Browsable = false }));
            _sprintSpeedConfig = config.Bind("Move", "Sprint Speed", 10f, new ConfigDescription("Default Sprint Speed", null, new ConfigurationManagerAttributes { Browsable = false }));
            _airSpeedConfig = config.Bind("Move", "Air Speed", 3f, new ConfigDescription("Default Air Speed", null, new ConfigurationManagerAttributes { Browsable = false }));
            _crouchSpeedConfig = config.Bind("Move", "Crouch Speed", 4f, new ConfigDescription("Default Crouch Speed", null, new ConfigurationManagerAttributes { Browsable = false }));

            // 临时变量存储配置初始值
            _tempWalkSpeed = _walkSpeedConfig.Value;
            _tempSprintSpeed = _sprintSpeedConfig.Value;
            _tempAirSpeed = _airSpeedConfig.Value;
            _tempCrouchSpeed = _crouchSpeedConfig.Value;
        }

        // 控制窗口的可见性
        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        // 绘制窗口
        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(2, _windowRect, DrawWindowContent, "Move Speed Manager");

                // 窗口在屏幕中央
                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2+30;
            }
        }

        // 绘制窗口内容
        private void DrawWindowContent(int windowID)
        {
            // Walk Speed
            GUILayout.Label($"Current Walk Speed: {_tempWalkSpeed:F2}");
            _tempWalkSpeed = GUILayout.HorizontalSlider(_tempWalkSpeed, 5f, 20f);

            // Sprint Speed
            GUILayout.Label($"Current Sprint Speed: {_tempSprintSpeed:F2}");
            _tempSprintSpeed = GUILayout.HorizontalSlider(_tempSprintSpeed, 10f, 20f);

            // Air Speed
            GUILayout.Label($"Current Air Speed: {_tempAirSpeed:F2}");
            _tempAirSpeed = GUILayout.HorizontalSlider(_tempAirSpeed, 3f, 20f);

            // Crouch Speed
            GUILayout.Label($"Current Crouch Speed: {_tempCrouchSpeed:F2}");
            _tempCrouchSpeed = GUILayout.HorizontalSlider(_tempCrouchSpeed, 4f, 20f);

            if (GUILayout.Button("Confirm"))
            {
                ApplySpeedSettings();
            }

            GUI.DragWindow();
        }

        // 更新速度设置
        private void ApplySpeedSettings()
        {
            // 将临时值应用到配置文件
            _walkSpeedConfig.Value = _tempWalkSpeed;
            _sprintSpeedConfig.Value = _tempSprintSpeed;
            _airSpeedConfig.Value = _tempAirSpeed;
            _crouchSpeedConfig.Value = _tempCrouchSpeed;

            if (StarterAssets.FirstPersonController.Instance != null)
            {
                // 将速度值应用到游戏中
                StarterAssets.FirstPersonController.Instance.MoveSpeed = _walkSpeedConfig.Value;
                StarterAssets.FirstPersonController.Instance.SprintSpeed = _sprintSpeedConfig.Value;
                StarterAssets.FirstPersonController.Instance.airSpeed = _airSpeedConfig.Value;
                StarterAssets.FirstPersonController.Instance.CrouchSpeed = _crouchSpeedConfig.Value;

                _logger.LogInfo("Move speeds updated successfully.");
            }
            else
            {
                _logger.LogWarning("FirstPersonController.Instance is null, unable to apply move speeds.");
            }
        }
    }
}
