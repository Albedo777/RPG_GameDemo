using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ChangeDirection : Havior
{
    private UnitBase unit;
    
    public override ETaskResult Execute(ETaskResult result, bool last)
    {
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