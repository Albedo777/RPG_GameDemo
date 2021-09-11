using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Walk : Havior
{
    private UnitBase unit;
    private float timeInterval = 0.1f;
    private float timer = 0;

    public override ETaskResult Execute(ETaskResult result, bool last)
    {
        unit.Transmit(ETransmitType.AutoMove);
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