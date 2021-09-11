using UnityEngine;

/**
 * 战斗单位的数据类
 * @author ligzh
 * @date 2021/9/2
 */

public class UnitData : IClass
{
    public int id;
    public bool isDead;
    public float attack;
    public BattleUnit battleUnit;
    public Transform unitTrans;
    public Transform targetPosition;
    public float Hp;
    public string modelName;
    
    public void OnReset()
    {
        
    }
}
