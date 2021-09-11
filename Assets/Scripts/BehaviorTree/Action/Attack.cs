using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Havior
{
    private UnitBase unit;
    
    public override ETaskResult Execute(ETaskResult result, bool last)
    {
        unit.Transmit(ETransmitType.Attack);
        UiManager.Instance.DispatchEvent("ChangeHpDisplay", -unit.Data.attack * Time.deltaTime);
        UnitManager.Instance.GetPlayerUnit().Data.Hp -= unit.Data.attack;
        return ETaskResult.Successed;
    }

    public override void Parse(Dictionary<string, object> data)
    {
        
    }

    public override void SharedVariableChanged(object value)
    {
        unit = (UnitBase) value;
    }
}
