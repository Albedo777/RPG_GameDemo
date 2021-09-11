using UnityEngine;
using UnityEngine.UI;

/**
 * 世界坐标血条UI组件
 * @author ligzh
 * @date 2021/9/5
 */

public class HudComponent : IComponent, ITransmit
{
    private UnitBase unit;
    private Slider hpSlider;
    private float totalHp;
    private float currentHp;

    public void OnInitialize(UnitBase unit)
    {
        this.unit = unit;
    }

    public void OnBirth()
    {
        GameObject hudObj = ResourceManager.Instance.LoadGameObjectResource(FactoryType.UIFactory, "HudEnemy");
        hudObj.transform.SetParent(unit.Data.unitTrans);
        hudObj.transform.localPosition = Vector3.zero;
        hudObj.transform.localEulerAngles = Vector3.zero;
        hpSlider = hudObj.GetComponentInChildren<Slider>();
        totalHp = unit.Data.Hp;
        currentHp = totalHp;
        hpSlider.value = currentHp / totalHp;
    }

    public void OnDead()
    {
    }

    public void OnUpdate()
    {
    }

    public void OnTransmit(ETransmitType type, BaseTransmitArg arg)
    {
        if (type == ETransmitType.ChangeHp)
        {
            HpChangeArg hpArg = arg as HpChangeArg;
            currentHp += hpArg.changHpVal;
            hpSlider.value = currentHp / totalHp;
            if (currentHp <= 0)
            {
                unit.Data.isDead = true;
                unit.Transmit(ETransmitType.Dead);
                TimeManager.Instance.DelayCall(2000,
                    () =>
                    {
                        ResourceManager.Instance.FreeResource(FactoryType.GameFactory, unit.Data.modelName,
                            unit.gameObject);
                    });
            }
        }
    }
}