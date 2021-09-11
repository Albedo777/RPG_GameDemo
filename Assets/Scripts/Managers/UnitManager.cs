using System.Collections.Generic;

/**
 * 单位管理者
 * 管理玩家和敌人单位的引用
 * @author ligzh
 * @date 2021/9/4
 */

public class UnitManager : IClass
{
    private UnitBase playerUnit;
    private List<UnitBase> enemyUnitList = new List<UnitBase>();
    private static UnitManager instance;

    public static UnitManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UnitManager();
            }

            return instance;
        }
    }

    public UnitBase AddPlayer(UnitData data)
    {
        BattleUnit unit = new BattleUnit();
        playerUnit = unit;
        unit.Initialize(data);
        unit.Birth();
        return unit;
    }

    public UnitBase AddEnemyUnit(UnitData data)
    {
        BattleUnit unit = new BattleUnit();
        enemyUnitList.Add(unit);
        unit.Initialize(data);
        unit.Birth();
        return unit;
    }

    public UnitBase GetPlayerUnit()
    {
        return playerUnit;
    }

    public List<UnitBase> GetEnemyUnitList()
    {
        return enemyUnitList;
    }

    public void OnReset()
    {
    }
}