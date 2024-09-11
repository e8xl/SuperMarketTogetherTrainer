using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using HarmonyLib;

namespace SMTTrainer
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Supermarket Together.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance; // 添加静态实例引用

        private ConfigEntry<bool> _isGoldWindowEnabled;
        private ConfigEntry<bool> _isPointWindowEnabled;
        private ConfigEntry<bool> _isEmployeesWindowEnabled;
        private ConfigEntry<bool> _isCheckoutWindowEnabled;
        private ConfigEntry<bool> _isCostWindowEnabled;
        private ConfigEntry<bool> _isGlobalWindowEnabled;
        private ConfigEntry<bool> _isFunWindowEnabled;
        private ConfigEntry<bool> _isMoveWindowEnabled;
        //private ConfigEntry<bool> _isTestWindowEnabled;


        private GoldManager _goldManager;
        private PointManager _pointManager;
        private EmployeesManager _employeesManager;
        private CheckoutManager _checkoutManager;
        private CostManager _costManager;
        public GlobalManager GlobalManager { get; private set; }
        public FunManager FunManager { get; private set; }
        public MoveManager MoveManager { get; private set; }
        //public TestManager TestManager { get; private set; }

        private Harmony _harmony;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // 注册goldManager, pointManager, employeesManager, checkoutManager, costManager, globalManager
            _goldManager = new GoldManager(Config, Logger);
            _pointManager = new PointManager(Config, Logger);
            _employeesManager = new EmployeesManager(Config, Logger);
            _checkoutManager = new CheckoutManager(Config, Logger);
            _costManager = new CostManager(Config, Logger);
            GlobalManager = new GlobalManager(Config, Logger);
            FunManager = new FunManager(Config, Logger);
            MoveManager = new MoveManager(Config, Logger);
            //TestManager = new TestManager(Config, Logger);

            // 绑定配置以显示Gold窗口
            _isGoldWindowEnabled = Config.Bind<bool>(
                "Trainer",
                "Gold Trainer",
                false,
                "Enable or disable the display of the Gold Trainer window.");

            // 绑定配置以显示Point窗口
            _isPointWindowEnabled = Config.Bind<bool>(
                "Trainer",
                "Point Trainer",
                false,
                "Enable or disable the display of the Point Trainer window.");

            // 绑定配置以显示Employees窗口
            _isEmployeesWindowEnabled = Config.Bind<bool>(
                "Trainer",
                "Employees Trainer(Host)",
                false,
                "Enable or disable the display of the Employees Trainer window.");

            // 绑定配置以显示Checkout窗口
            _isCheckoutWindowEnabled = Config.Bind<bool>(
                "Trainer",
                "Checkout Trainer(Host)",
                false,
                "Enable or disable the display of the Checkout Trainer window.");

            // 绑定配置以显示Cost窗口
            _isCostWindowEnabled = Config.Bind<bool>(
                "Trainer",
                "Cost Trainer(Maybe Not Run!!)",
                false,
                "Maybe Not Run!!! Enable or disable the display of the Cost Trainer window.");

            // 绑定配置以显示Global窗口
            _isGlobalWindowEnabled = Config.Bind<bool>(
                "Trainer",
                "Global Time Trainer(Host)",
                false,
                "Enable or disable the display of the Global Time Trainer window.");

            // 绑定配置以显示 Fun 窗口
            _isFunWindowEnabled = Config.Bind<bool>(
                "Trainer",
                "Fun Manager(Host)",
                false,
                "Enable or disable the display of the Fun Manager window.");

            // 绑定配置以显示 Move 窗口
            _isMoveWindowEnabled = Config.Bind<bool>(
                "Trainer",
                "Move Manager",
                false,
                "Enable or disable the display of the Move Manager window.");

            // 绑定配置以显示 Test 窗口
            //_isTestWindowEnabled = Config.Bind<bool>(
            //    "Trainer",
            //    "Enable Test Manager(Only for test|Can't Running!!!)",
            //    false,
            //    "Enable or disable the display of the Test Manager window.");


            Instance = this;

            // 设置配置更改事件
            _isGoldWindowEnabled.SettingChanged += OnGoldWindowEnableChanged;
            _isPointWindowEnabled.SettingChanged += OnPointWindowEnableChanged;
            _isEmployeesWindowEnabled.SettingChanged += OnEmployeesWindowEnableChanged;
            _isCheckoutWindowEnabled.SettingChanged += OnCheckoutWindowEnableChanged;
            _isCostWindowEnabled.SettingChanged += OnCostWindowEnableChanged;
            _isGlobalWindowEnabled.SettingChanged += OnGlobalWindowEnableChanged;
            _isFunWindowEnabled.SettingChanged += OnFunWindowEnableChanged;
            _isMoveWindowEnabled.SettingChanged += OnMoveWindowEnableChanged;
            //_isTestWindowEnabled.SettingChanged += OnTestWindowEnableChanged;
            // 注册Harmony补丁
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
        }

        private void OnGoldWindowEnableChanged(object sender, System.EventArgs e)
        {
            _goldManager.SetWindowVisibility(_isGoldWindowEnabled.Value);
        }

        private void OnPointWindowEnableChanged(object sender, System.EventArgs e)
        {
            _pointManager.SetWindowVisibility(_isPointWindowEnabled.Value);
        }

        private void OnEmployeesWindowEnableChanged(object sender, System.EventArgs e)
        {
            _employeesManager.SetWindowVisibility(_isEmployeesWindowEnabled.Value);
        }

        private void OnCheckoutWindowEnableChanged(object sender, System.EventArgs e)
        {
            _checkoutManager.SetWindowVisibility(_isCheckoutWindowEnabled.Value);
        }

        private void OnCostWindowEnableChanged(object sender, System.EventArgs e)
        {
            _costManager.SetWindowVisibility(_isCostWindowEnabled.Value);
        }

        private void OnGlobalWindowEnableChanged(object sender, System.EventArgs e)
        {
            GlobalManager.SetWindowVisibility(_isGlobalWindowEnabled.Value);
        }

        private void OnFunWindowEnableChanged(object sender, System.EventArgs e)
        {
            FunManager.SetWindowVisibility(_isFunWindowEnabled.Value); // 控制 FunManager 窗口显示
        }
        private void OnMoveWindowEnableChanged(object sender, System.EventArgs e)
        {
            MoveManager.SetWindowVisibility(_isMoveWindowEnabled.Value);
        }
        //private void OnTestWindowEnableChanged(object sender, System.EventArgs e)
        //{
        //    TestManager.SetWindowVisibility(_isTestWindowEnabled.Value); // 控制 TestManager 窗口显示
        //}

        private void OnGUI()
        {
            // 绘制Gold窗口
            if (_isGoldWindowEnabled.Value)
            {
                _goldManager.DrawWindow();
            }

            // 绘制Point窗口
            if (_isPointWindowEnabled.Value)
            {
                _pointManager.DrawWindow();
            }

            // 绘制Employees窗口
            if (_isEmployeesWindowEnabled.Value)
            {
                _employeesManager.DrawWindow();
            }

            // 绘制Checkout窗口
            if (_isCheckoutWindowEnabled.Value)
            {
                _checkoutManager.DrawWindow();
            }

            // 绘制Cost窗口
            if (_isCostWindowEnabled.Value)
            {
                _costManager.DrawWindow();
            }

            // 绘制Global窗口
            if (_isGlobalWindowEnabled.Value)
            {
                GlobalManager.DrawWindow();
            }

            // 绘制 Fun 窗口
            if (_isFunWindowEnabled.Value)
            {
                FunManager.DrawWindow();
            }

            // 绘制 Move 窗口
            if (_isMoveWindowEnabled.Value)
            {
                MoveManager.DrawWindow();
            }
            //// 绘制 Test 窗口
            //if (_isTestWindowEnabled.Value)
            //{
            //    TestManager.DrawWindow(); // 绘制 Test 窗口
            //}
        }

        private void Update()
        {
            // 每帧检测 实例 是否已经存在
            _employeesManager.Update();
            _checkoutManager.Update();
            _costManager.Update();
            GlobalManager.Update();
            FunManager.Update();
    }
    }
}
