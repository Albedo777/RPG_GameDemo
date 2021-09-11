using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanAttack : Havior
{
    private UnitBase unit;

    public override ETaskResult Execute(ETaskResult result, bool last)
    {
        if (Math.Abs(Vector3.Distance(unit.Data.unitTrans.position,
            UnitManager.Instance.GetPlayerUnit().Data.unitTrans.position)) < 1)
        {
            return ETaskResult.Successed;
        }
        else
        {
            return ETaskResult.Failed;
        }
    }

    public override void Parse(Dictionary<string, object> data)
    {
    }

    public override void SharedVariableChanged(object value)
    {
        unit = (UnitBase) value;
    }
}