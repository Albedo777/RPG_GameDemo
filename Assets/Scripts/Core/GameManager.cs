using System.Collections.Generic;
using UnityEngine;

/**
 * 游戏管理者
 * 继承 Monobehaviour
 * @author ligzh
 * @date 2021/9/3
 */

public class GameManager : MonoBehaviour, IClass
{
    private UnitBase unit;
    private ResourceManager resourceManager;
    private TimeManager timeManager;
    private UiManager uiManager;
    private LuaManager luaManager;
    private static GameManager instance;
    public static GameManager Instance => instance;

    private void Awake()
    {
        ClassManager.Instance.Store<ResourceManager>(1);
        ClassManager.Instance.Store<UiManager>(1);
        ClassManager.Instance.Store<LuaManager>(1);

        instance = this;
        resourceManager = ResourceManager.Instance;
        resourceManager.Init();
        resourceManager.PreLoadGameObjectResource(FactoryType.GameFactory, "Skill1008", 5);
        resourceManager.PreLoadGameObjectResource(FactoryType.GameFactory, "Skill1009", 5);
        resourceManager.PreLoadGameObjectResource(FactoryType.GameFactory, "Skill1010", 5);
        resourceManager.PreLoadGameObjectResource(FactoryType.GameFactory, "Skill1011", 5);
        resourceManager.PreLoadGameObjectResource(FactoryType.GameFactory, "Skill1012", 5);

        TimeManager.GlobalTimeScale = 1;
        TimeManager.GameTimeScale = 1;
        timeManager = TimeManager.Instance;

        uiManager = UiManager.Instance;
        uiManager.Init();

        luaManager = LuaManager.Instance;
        luaManager.Init();
    }

    private void Start()
    {
        //添加玩家
        unit = UnitManager.Instance.AddPlayer(GameUtil.LoadPlayerConfig());
        //UI配置
        var uiConfigs = GameUtil.LoadUIinfo();
        foreach (var uiConfig in uiConfigs)
        {
            uiManager.LoadUIConfig(uiConfig);
        }

        uiManager.OpenUI("CardUI");
        uiManager.OpenUI("TopUI");
        //卡牌设置
        List<SkillCardConfig> configs = GameUtil.LoadCardConfig();
        //卡牌初始化
        CardManager.Instacne.Init((BattleUnit) unit, configs);
        //敌人加入测试
        var enemyConfigs = GameUtil.LoadEnemyConfigs();
        for (int i = 0; i < enemyConfigs.Count; i++)
        {
            UnitBase enemy = UnitManager.Instance.AddEnemyUnit(enemyConfigs[i]);
            enemy.Data.unitTrans.position = new Vector3(-10 + 5 * i, 0, 10);
        }
    }

    void Update()
    {
        unit.Update();
        timeManager.Update();
        uiManager.Update();
        luaManager.Update();
        foreach (var enemy in UnitManager.Instance.GetEnemyUnitList())
        {
            enemy.Update();
        }
    }

    public void OnReset()
    {
        ClassManager.Instance.Free(this);
        timeManager = null;
        uiManager = null;
        luaManager = null;
        resourceManager = null;
        unit = null;
    }
}