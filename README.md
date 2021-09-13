# RPG_GameDemo
A simple RPG demo based on the ECS framework with Unity

RPG游戏Demo:自实现的简单的 RPG 游戏示例。

整体使用 Manager of manager 框架，使用继承 Monobehaviour 的 GameManager 管理其他 managers:
* ClassManager 结合 ClassPool 管理类的初始化和回收;
* TimeManager 控制游戏的延 时调用和暂停;
* ResourceManager 结合对象池技术控制游戏资源的预加载和回收;
* UiManager 控制继承 UIBase 的UI脚本之 间的消息注册和发送、层级管理和打开关闭的方法的统一调用;
* LuaManager 基于xlua和反射，可以将Lua配置文件的数据读 入数据类中(Lua配置文件由自编写的Unity拓展 Excel转换工具导入);

战斗单位基于ECS框架，角色拥有移动组件、技能释放 组件、AI组件(基于代码实现的简单行为树)和血条组件，组件之间的交互通过消息机制。

此外，游戏中还实现了一些简单 的shader，如怪物的反射材质，场景中的水面效果。

## 游戏效果演示
* 技能释放器
* 技能卡牌拖动
* 开牌UI，头像UI，暂停UI
* 敌人反射材质，AI攻击
![image](https://github.com/Albedo777/RPG_GameDemo/blob/main/Demo.png)

## 配置工具
![image](https://github.com/Albedo777/RPG_GameDemo/blob/main/Editor.png)

## 行为树
![image](https://github.com/Albedo777/RPG_GameDemo/blob/main/BehaviorTree.png)
