using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using System.Collections;
using System;
using System.Reflection;

namespace SMTTrainer
{
    public class FunManager
    {
        public readonly ConfigEntry<bool> _disableTrashGenerationConfig; // 复选框配置
        private readonly ManualLogSource _logger;
        private Rect _windowRect = new Rect(0, 0, 300, 150);
        private bool _showWindow;

        public FunManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;

            // 配置复选框是否禁用垃圾生成
            _disableTrashGenerationConfig = config.Bind<bool>(
                "Trash Settings(Only Host!!!)",
                "Disable Trash Generation",
                false,
                new ConfigDescription("Enable or disable automatic trash generation in the game.", null, new ConfigurationManagerAttributes { Browsable = false }));
        }

        public void SetWindowVisibility(bool visible)
        {
            _showWindow = visible;
        }

        public void DrawWindow()
        {
            if (_showWindow)
            {
                _windowRect = GUILayout.Window(6, _windowRect, DrawWindowContent, "Fun Manager");

                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        private void DrawWindowContent(int windowID)
        {
            _disableTrashGenerationConfig.Value = GUILayout.Toggle(_disableTrashGenerationConfig.Value, "Disable Trash Generation|Only Host!!!");

            if (GUILayout.Button("Generate Random Trash (1-5)|Only Host!!!"))
            {
                GenerateRandomTrash();
            }

            GUI.DragWindow();
        }

        // 强制生成 1-5 个垃圾
        private void GenerateRandomTrash()
        {
            if (GameData.Instance != null)
            {
                int trashCount = UnityEngine.Random.Range(1, 6); // 随机生成1到5个垃圾
                for (int i = 0; i < trashCount; i++)
                {
                    StartCoroutineForceSpawnTrash();
                }

                _logger.LogInfo($"Generated {trashCount} pieces of trash.");
            }
            else
            {
                _logger.LogWarning("GameData.Instance is null, can't generate trash.");
            }
        }

        // 使用反射来调用私有的 SpawnTrash 方法
        private void StartCoroutineForceSpawnTrash()
        {
            if (GameData.Instance != null)
            {
                // 通过反射找到 SpawnTrash 方法
                MethodInfo spawnTrashMethod = typeof(GameData).GetMethod("SpawnTrash", BindingFlags.NonPublic | BindingFlags.Instance);

                if (spawnTrashMethod != null)
                {
                    // 调用私有的 SpawnTrash 方法
                    IEnumerator spawnTrashCoroutine = (IEnumerator)spawnTrashMethod.Invoke(GameData.Instance, null);
                    Plugin.Instance.StartCoroutine(spawnTrashCoroutine);
                }
                else
                {
                    _logger.LogError("Failed to find SpawnTrash method via reflection.");
                }
            }
        }

        public void Update() { }
    }

    // 使用 Harmony 对 GameData.TrashManager 方法进行 Hook
    [HarmonyPatch(typeof(GameData), "TrashManager")]
    public static class TrashManagerPatch
    {
        static bool Prefix()
        {
            // 如果勾选了禁止垃圾生成的复选框，则阻止原 TrashManager 方法执行
            if (Plugin.Instance.FunManager._disableTrashGenerationConfig.Value)
            {
                return false; // 阻止原始垃圾生成逻辑
            }

            return true; // 允许正常执行原始方法
        }
    }
}
