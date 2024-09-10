# SuperMarketTogetherTrainer
## 注意事项
### 请及时备份存档
### 一切修改都有损坏存档的可能性 请谨慎修改
1. 每次进入存档都会读取Employees的设置 因为每次进入存档都会重新生成(Spawn,模型实体)员工 生成员工数量等于已解锁特许经营点数员工数量+EmployeesTrainer参数设置数量
2. 当局内在修改参数之前已经生成的员工无法修改速度 只有新生成的可以修改
3. 已经生成的员工无法删除 调用的方法会重新设置员工总数 需要存档（结束一天）才能保存员工数量
4. 已知供货员过多情况下 因游戏NPC寻路规则不稳定 会导致很多NPC卡在一起 ~~转圈~~
5. 员工速度过快会导致寻路异常（绕远路 撞墙等等） 
6. 默认EmployeesSetting MaxEmployees为3 Speed为0.1f 不修改的情况下进入游戏 默认为3+已经解锁特许经营点数员工数量<br>速度为0.1游戏默认倍速
### 暂未测试游戏局内修改员工参数存档后数据情况 请谨慎修改避免坏档

---
### 说明
1. 因为这游戏用CE改起来很容易 ~~崩溃~~（wobuhuizhaojizhi) 导致每次打开ct改起来很不舒服 就连夜学了BepInEx和C# 写了这个并不是很好的MOD
2. 代码基本上 ~~（全部）~~ 都是ChatGPT写的 我提供了思路和逻辑 基本上用dnSPY找到方法就很简单了
   1. 我实在是不知道怎么在BepInEX.configurationManager里面显示中文了 中文打进去就是乱码
   2. 所以我做了两个分支 但Chinese分支在我研究明白之前很难更新了
3. 功能基本上就这些了 这个游戏能改的部分并不是太多了吧 有什么思路的话可以发issue喵
4. 英语水平极低 大多数靠DeepL和ChatGPT 但应该能看懂吧 毕竟也没几个单词......
---
### 教程/Chinese Tutorial
1. 装[BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip)到游戏里面
2. 启动游戏
3. 装[ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/download/v18.3/BepInEx.ConfigurationManager.BepInEx5_v18.3.zip)到Plugin里面
4. 把发布的DLL（或包含DLL文件的同名文件夹）放到BepInEx/Plugin里面
5. 启动游戏按F1 即可进行配置

### English Tutorial
1. Install [BepInEx](https://github.com/BepInEx/BepInEx/releases/download/v5.4.23.2/BepInEx_win_x64_5.4.23.2.zip) into the game.
2. Launch the game.
3. Install [BepInEx.ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/download/v18.3/BepInEx.ConfigurationManager.BepInEx5_v18.3.zip) into the Plugins folder.
4. Place the released DLL (or the folder with the same name that contains the DLL) into the BepInEx/Plugins folder.
5. Start the game and press F1 to access the configuration.
---
### All modifications may potentially corrupt your save file, so please proceed with caution.
## Please remember to back up your save files regularly.
1. Each time you enter a save, the settings for Employees are loaded. This is because entering a save will respawn the employees (models/entities). The number of employees generated equals the number of employees unlocked via Franchise Points plus the number set in the EmployeesTrainer parameters.

2. Employees that were generated before changing parameters during a session cannot have their speed modified. Only newly spawned employees can have their speed altered.

3. Already generated employees cannot be deleted. The method used will reset the total number of employees, but you will need to save the game (by ending the day) to retain the updated employee count.

4. It is known that having too many suppliers can cause issues due to unstable NPC pathfinding rules in the game, leading to NPCs getting stuck together or "spinning in circles."

5. Setting the employee speed too high may cause pathfinding problems (e.g., taking long detours, running into walls, etc.).

6. By default, the EmployeesSetting MaxEmployees is set to 3 and Speed is set to 0.1f. If you do not modify these settings, the game will default to 3 employees plus the number unlocked with Franchise Points. The speed will be 0.1, which is the game's default speed multiplier.

### The effects of modifying employee parameters mid-session and then saving the game have not been fully tested. Please proceed with caution to avoid save file corruption.