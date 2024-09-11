using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace SMTTrainer
{
    public class TestManager
    {
        private readonly ConfigEntry<int> _productIDConfig;
        private readonly ManualLogSource _logger;
        private Rect _windowRect = new Rect(0, 0, 300, 200);
        private bool _showWindow;
        private string _tempProductID;

        private readonly int _minProductID = 0;
        private readonly int _maxProductID = 175;

        public TestManager(ConfigFile config, ManualLogSource logger)
        {
            _logger = logger;

            // 配置项，范围为 0 - 175
            _productIDConfig = config.Bind<int>(
                "AutoFill",
                "Product ID",
                0,
                new ConfigDescription($"Product ID to add (Range: {_minProductID} - {_maxProductID})",
                                      new AcceptableValueRange<int>(_minProductID, _maxProductID),
                                      new ConfigurationManagerAttributes { Browsable = false }));

            _tempProductID = _productIDConfig.Value.ToString();
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
                _windowRect = GUILayout.Window(1, _windowRect, DrawWindowContent, "Test Trainer");

                // 窗口在屏幕中央
                _windowRect.x = (Screen.width - _windowRect.width) / 2;
                _windowRect.y = (Screen.height - _windowRect.height) / 2;
            }
        }

        // 绘制窗口内容
        private void DrawWindowContent(int windowID)
        {
            GUILayout.Label($"Current Product ID: {_productIDConfig.Value}");
            GUILayout.Label($"Enter Product ID (Range: {_minProductID}-{_maxProductID}):");

            _tempProductID = GUILayout.TextField(_tempProductID);

            if (GUILayout.Button("Submit"))
            {
                if (int.TryParse(_tempProductID, out int newProductID))
                {
                    if (newProductID >= _minProductID && newProductID <= _maxProductID)
                    {
                        SetProductID(newProductID);
                        _productIDConfig.Value = newProductID;
                    }
                    else
                    {
                        _logger.LogError($"Invalid Product ID! Please enter a value between {_minProductID} and {_maxProductID}.");
                    }
                }
                else
                {
                    _logger.LogError("Invalid input! Please enter a valid integer for the product ID.");
                }
            }

            GUI.DragWindow();
        }

        // 设置产品 ID 并调用 AddProduct
        private void SetProductID(int newProductID)
        {
            var debugAutoFill = Object.FindFirstObjectByType<DEBUG_AutoFill>(); // 修改为新的API
            if (debugAutoFill != null)
            {
                // 通过 Harmony 来修改 DEBUG_AutoFill 的 productID 并调用 AddProduct 方法
                Patch_DebugAutoFill.ProductID = newProductID;

                // Harmony 补丁将处理 AddProduct 调用
                _logger.LogInfo($"Product with ID {newProductID} added successfully.");
            }
            else
            {
                _logger.LogError("DEBUG_AutoFill instance not found.");
            }
        }
    }

    // Harmony 补丁类
    [HarmonyPatch(typeof(DEBUG_AutoFill), "Update")]
    public static class Patch_DebugAutoFill
    {
        public static int ProductID;

        // 在 Update 方法执行时，将自动更新 productID
        static void Prefix(DEBUG_AutoFill __instance)
        {
            __instance.productID = ProductID;
            // 假设 AddProduct 是私有或受保护的方法，这里可以利用 Harmony 来绕过它的访问权限
            AccessTools.Method(typeof(DEBUG_AutoFill), "AddProduct").Invoke(__instance, null);
        }
    }
}
