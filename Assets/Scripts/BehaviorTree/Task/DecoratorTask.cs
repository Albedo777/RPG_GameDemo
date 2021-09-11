using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecoratorTask : ITask
{
    public int childTask;
    int ITask.id { get; set; }
    int ITask.parentIndex { get; set; }
    int ITask.childIndex { get; set; }
    public abstract ETaskResult Execute(ETaskResult result, bool last);
    ETaskType ITask.taskType { get { return ETaskType.Decorator; } set { } }
    public abstract void Parse(Dictionary<string, object> data);
    public abstract void SharedVariableChanged(string key, object value);
    public abstract void Ready();
    public abstract void End();
    public abstract void OnReset();
}
