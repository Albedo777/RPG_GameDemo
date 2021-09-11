using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : CompositeTask
{
    public override ETaskResult Execute(ETaskResult result, bool last)
    {
        if (last)
        {
            return result == ETaskResult.Failed ? ETaskResult.Failed : ETaskResult.Successed;
        }
        else
        {
            return result == ETaskResult.Failed ? ETaskResult.Failed : ETaskResult.Running;
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