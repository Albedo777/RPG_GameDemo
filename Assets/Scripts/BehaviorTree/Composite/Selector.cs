using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : CompositeTask
{
    public override ETaskResult Execute(ETaskResult result, bool last)
    {
        if (last)
        {
            return result == ETaskResult.Successed ? ETaskResult.Successed : ETaskResult.Failed;
        }
        else
        {
            return result == ETaskResult.Successed ? ETaskResult.Successed : ETaskResult.Running;
        }
    }

    public override void Ready()
    {
    }

    public override void End()
    {
    }

    public override void OnReset()
    {
    }
}