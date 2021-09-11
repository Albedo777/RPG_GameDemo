using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LeafTask : ITask
{
    int ITask.id { get; set; }
    int ITask.parentIndex { get; set; }
    int ITask.childIndex { get; set; }
    public abstract ETaskResult Execute(ETaskResult result, bool last);
    ETaskType ITask.taskType { get { return ETaskType.Leaf; } set { } }
    public abstract void Parse(Dictionary<string, object> data);
    public abstract void SharedVariableChanged(object value);
    public abstract void Ready();
    public abstract void End();
}
