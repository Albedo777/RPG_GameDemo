using System.Collections.Generic;

/**
 * 游戏配置的加载
 * @author ligzh
 * @date 2021/9/9
 */


public static class GameUtil
{
    private static Dictionary<int, SkillCardConfig> skillCardConfig;
    private static Dictionary<int, UnitData> battleUnitConfig;
    
    public static List<SkillCardConfig> LoadCardConfig()
    {
        if (skillCardConfig == null)
        {
            skillCardConfig = LuaManager.Instance.LoadConfigFile<SkillCardConfig>("skillcard");
        }
        List<SkillCardConfig> configs = new List<SkillCardConfig>();
        foreach (var config in skillCardConfig.Values)
        {
            configs.Add(config);
        }
        return configs;
    }
    
    public static UnitData LoadPlayerConfig()
    {
        if (battleUnitConfig == null)
        {
            battleUnitConfig = LuaManager.Instance.LoadConfigFile<UnitData>("battleUnit");
        }

        return battleUnitConfig[1];
    }

    public static List<UnitData> LoadEnemyConfigs()
    {
        if (battleUnitConfig == null)
        {
            battleUnitConfig = LuaManager.Instance.LoadConfigFile<UnitData>("battleunit");
        }
        List<UnitData> configs = new List<UnitData>();
        foreach (var config in battleUnitConfig)
        {
            if (config.Key > 1)
            {
                configs.Add(config.Value);
            }
        }
        
        return configs;
    }

    public static List<UIInfo> LoadUIinfo()
    {
        List<UIInfo> configs = new List<UIInfo>();
        configs.Add(new UIInfo()
        {
            layer =  UILyaer.Bottom,
            name = "CardUI"
        });
        configs.Add(new UIInfo()
        {
            layer = UILyaer.Top,
            name = "TopUI"
        });
        configs.Add(new UIInfo()
        {
            layer = UILyaer.Window,
            name = "PauseUI"
        });
        return configs;
    }
}
