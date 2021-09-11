using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree : IClass
{
    private int[] m_taskKeys;
    private ITask[] m_taskValues; //任务id
    private ITask m_runningTask;
    private List<int> m_nodeTree = new List<int>(); //节点id: 1 - (5) 12 8 9 2 3; 9 - (2) 10 11; 2 - (2) 4 5; 3 - (2) 6 7 
    private int[] m_nodeTreeArr;
    private ITask m_firstExeNode;

    public void Initialize(BehaviorData data)
    {
        int taskNum = data.tasks.Length;
        m_taskKeys = new int[taskNum + 1];
        m_taskValues = new ITask[taskNum + 1];
        for (int i = 0; i < taskNum; i++)
        {
            BehaviorData.TaskData taskData = data.tasks[i];
            ITask curTask = (ITask) Activator.CreateInstance(Type.GetType(taskData.className));
            curTask.id = taskData.id;
            curTask.taskType = (ETaskType) taskData.taskType;
            if (curTask.taskType == ETaskType.Composite)
            {
                ((CompositeTask) curTask).tasks = data.composites[taskData.id];
            }
            else if (curTask.taskType == ETaskType.Decorator)
            {
                ((DecoratorTask) curTask).childTask = data.decorators[taskData.id];
                ((DecoratorTask) curTask).Parse(taskData.param);
            }
            else
            {
                ((LeafTask) curTask).Parse(taskData.param);
            }

            m_taskKeys[i + 1] = taskData.id;
            m_taskValues[i + 1] = curTask;
        }

        AddCompositeNode(data.rootId, -1, data.composites[data.rootId]);
        foreach (var sharedVariable in data.sharedVariables)
        {
            SetVariable(sharedVariable);
        }

        m_nodeTreeArr = m_nodeTree.ToArray();
        m_nodeTree.Clear();
    }

    public void Update()
    {
        if (m_runningTask == null)
        {
            ExecuteLeaf(m_firstExeNode);
        }
        else
        {
            ETaskResult result = m_runningTask.Execute(ETaskResult.Successed, false);
            if (result != ETaskResult.Running)
            {
                int compositeId = m_nodeTreeArr[m_runningTask.parentIndex];
                //int childLength = m_nodeTreeArr[m_runningTask.parentIndex + 1];
                ITask parentTask = m_taskValues[compositeId];
                if (parentTask is CompositeTask)
                {
                    ToExecuteComposite(m_runningTask.parentIndex, m_runningTask.childIndex, parentTask, result);
                }
                else if (parentTask is DecoratorTask)
                {
                    ToExecuteDecorator(m_runningTask.parentIndex, parentTask, result);
                }
                else
                {
                    Debug.LogError("leaf parent must Composite or Decorator");
                }

                m_runningTask.End();
                m_runningTask = null;
            }
        }
    }

    public void SetVariable(object value)
    {
        for (int i = 0; i < m_taskValues.Length; i++)
        {
            if (m_taskValues[i] is LeafTask)
            {
                ((LeafTask) m_taskValues[i]).SharedVariableChanged(value);
            }
        }
    }

    // private void AddCompositeNode(int taskId, int parentNodeId, int[] children)
    // {
    //     m_nodeTree.Add(taskId); //加入当前任务节点id
    //     m_taskValues[taskId].parentIndex = parentNodeId; //设置任务节点对应的Task的父节点索引
    //     int curNodeId = m_nodeTree.Count - 1;
    //     m_nodeTree.Add(children.Length); //加入子节点个数
    //
    //     for (int i = 0; i < children.Length; i++)
    //     {
    //         int childTaskId = children[i];
    //         m_nodeTree.Add(childTaskId);
    //         ITask childTask = m_taskValues[childTaskId];
    //         childTask.parentIndex = curNodeId;
    //         childTask.childIndex = i;
    //         if (childTask is CompositeTask)
    //         {
    //             //如果子任务是一个组合任务，继续添加组合节点和它的子节点
    //             AddCompositeNode(childTaskId, curNodeId, ((CompositeTask) childTask).tasks);
    //         }
    //         else if (childTask is DecoratorTask)
    //         {
    //             AddDecoratorNode(childTaskId, curNodeId, ((DecoratorTask) childTask).childTask);
    //         }
    //         else
    //         {
    //             if (m_firstExeNode == null)
    //             {
    //                 m_firstExeNode = childTask;
    //             }
    //         }
    //     }
    // }
    void AddCompositeNode(int taskId, int parentNodeId, int[] childs)
    {
        m_nodeTree.Add(taskId);
        m_taskValues[taskId].parentIndex = parentNodeId;
        int nodeId = m_nodeTree.Count - 1;
        m_nodeTree.Add(childs.Length);
        for (int i = 0; i < childs.Length; i++)
        {
            m_nodeTree.Add(childs[i]);
        }

        for (int i = 0; i < childs.Length; i++)
        {
            int c = childs[i];
            ITask task = m_taskValues[c];
            task.parentIndex = nodeId;
            task.childIndex = i;
            if (task is CompositeTask)
            {
                AddCompositeNode(c, nodeId, ((CompositeTask) task).tasks);
            }
            else if (task is DecoratorTask)
            {
                AddDecoratorNode(c, nodeId, ((DecoratorTask) task).childTask);
            }
            else
            {
                if (m_firstExeNode == null)
                {
                    m_firstExeNode = task;
                }
            }
        }
    }

    private void AddDecoratorNode(int taskId, int parentNodeId, int child)
    {
        m_nodeTree.Add(taskId);
        int curNodeId = m_nodeTree.Count - 1;
        ITask task = m_taskValues[child];
        task.parentIndex = curNodeId;
        task.childIndex = 0;
        if (task is CompositeTask)
        {
            AddCompositeNode(child, curNodeId, ((CompositeTask) task).tasks);
        }
        else if (task is DecoratorTask)
        {
            AddDecoratorNode(child, curNodeId, ((DecoratorTask) task).childTask);
        }
        else
        {
            if (m_firstExeNode == null)
            {
                m_firstExeNode = task;
            }
        }
    }

    private void ExecuteLeaf(ITask task)
    {
        task.Ready();
        ETaskResult result = task.Execute(ETaskResult.Successed, false);
        if (result == ETaskResult.Running)
        {
            m_runningTask = task;
        }
        else
        {
            int compositeId = m_nodeTreeArr[task.parentIndex];
            ITask parentTask = m_taskValues[compositeId];
            if (parentTask is CompositeTask)
            {
                ToExecuteComposite(task.parentIndex, task.childIndex, parentTask, result);
            }
            else if (parentTask is DecoratorTask)
            {
                ToExecuteDecorator(task.parentIndex, parentTask, result);
            }
            else
            {
                Debug.LogError("leaf parent must Composite or Decorator");
            }

            task.End();
        }
    }

    void ToExecuteComposite(int nodeId, int indexOfChild, ITask task, ETaskResult r)
    {
        int childLength = m_nodeTreeArr[nodeId + 1];
        bool last = indexOfChild == childLength - 1;
        task.Ready();
        ETaskResult result = task.Execute(r, last);
        if (result == ETaskResult.Running)
        {
            int childNodeId = nodeId + 1 + indexOfChild + 2;
            int childTaskId = m_nodeTreeArr[childNodeId];
            ITask childTask = m_taskValues[childTaskId];
            if (childTask.taskType == ETaskType.Composite)
            {
                ExecuteCompositeTo(childNodeId, childTask);
            }
            else if (childTask.taskType == ETaskType.Decorator)
            {
                ExecuteDecoratorTo(childNodeId, childTask);
            }
            else
            {
                ExecuteLeaf(childTask);
            }
        }
        else
        {
            if (task.parentIndex >= 0)
            {
                int compositeId = m_nodeTreeArr[task.parentIndex];
                ITask parentTask = m_taskValues[compositeId];
                ToExecuteComposite(task.parentIndex, task.childIndex, parentTask, result);
            }

            task.End();
        }
    }

    private void ExecuteCompositeTo(int nodeId, ITask task)
    {
        task.Ready();
        if (task.taskType == ETaskType.Composite)
        {
            CompositeTask comTask = (CompositeTask) task;
            int childCompositeId = comTask.tasks[0];
            ITask childTask = m_taskValues[childCompositeId];
            int childNodeId = childTask.parentIndex + 1 + 1;
            if (childTask.taskType == ETaskType.Composite)
            {
                ExecuteCompositeTo(childNodeId, childTask);
            }
            else if (childTask.taskType == ETaskType.Decorator)
            {
                ExecuteDecoratorTo(childNodeId, childTask);
            }
            else
            {
                ExecuteLeaf(childTask);
            }
        }
        else if (task.taskType == ETaskType.Decorator)
        {
            ExecuteDecoratorTo(nodeId + 1 + 1, task);
        }
        else
        {
            ExecuteLeaf(task);
        }

        task.End();
    }

    private void ToExecuteDecorator(int nodeId, ITask task, ETaskResult r)
    {
        task.Ready();
        ETaskResult result = task.Execute(r, true);
        if (result == ETaskResult.Running)
        {
        }
        else
        {
            if (task.parentIndex >= 0)
            {
                int compositeId = m_nodeTreeArr[task.parentIndex];
                ITask parentTask = m_taskValues[compositeId];
                if (parentTask is CompositeTask)
                {
                    ToExecuteComposite(task.parentIndex, task.childIndex, parentTask, result);
                }
                else if (parentTask is DecoratorTask)
                {
                    ToExecuteDecorator(task.parentIndex, parentTask, result);
                }
            }

            task.End();
        }
    }

    private void ExecuteDecoratorTo(int nodeId, ITask t)
    {
        t.Ready();
        DecoratorTask task = t as DecoratorTask;
        ITask childTask = m_taskValues[task.childTask];
        if (childTask is CompositeTask)
        {
            ExecuteCompositeTo(task.childTask, childTask);
        }
        else if (childTask is DecoratorTask)
        {
            ExecuteDecoratorTo(task.childTask, childTask);
        }
        else
        {
            ExecuteLeaf(childTask);
        }

        t.End();
    }

    public void OnReset()
    {
        
    }
}