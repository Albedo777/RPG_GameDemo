using System.Collections.Generic;

/**
 * 基于行为树的AI组件
 * @author ligzh
 * @date 2021/9/4
 */

public class BehaviorComponent : IComponent, ITransmit, IClass
{
    private BehaviorTree behaviorTree;
    private UnitBase unit;

    public void OnInitialize(UnitBase unit)
    {
        this.unit = unit;
    }

    public void OnBirth()
    {
        behaviorTree = ClassManager.Instance.Get<BehaviorTree>();
        behaviorTree.Initialize(SetData());
        behaviorTree.SetVariable(unit);
    }

    public void OnDead()
    {
    }

    public void OnUpdate()
    {
        behaviorTree.Update();
    }

    public void OnTransmit(ETransmitType type, BaseTransmitArg arg)
    {
    }

    private BehaviorData SetData()
    {
        BehaviorData data = new BehaviorData();
        data.rootId = 1;
        data.composites = new Dictionary<int, int[]>();
        data.decorators = new Dictionary<int, int>();
        data.sharedVariables = new List<object>();

        data.composites.Add(1, new[] {2, 3});
        data.composites.Add(2, new[] {4, 5, 6, 7});
        data.tasks = new[]
        {
            new BehaviorData.TaskData
            {
                id = 1,
                taskType = 2,
                className = "Selector"
            },
            new BehaviorData.TaskData
            {
                id = 2,
                taskType = 2,
                className = "Sequence"
            },
            new BehaviorData.TaskData
            {
                id = 3,
                taskType = 1,
                className = "ChangeDirection"
            },
            new BehaviorData.TaskData
            {
                id = 4,
                taskType = 1,
                className = "CanWalk"
            },
            new BehaviorData.TaskData
            {
                id = 5,
                taskType = 1,
                className = "Walk"
            },
            new BehaviorData.TaskData
            {
                id = 6,
                taskType = 1,
                className = "CanAttack"
            },
            new BehaviorData.TaskData
            {
                id = 7,
                taskType = 1,
                className = "Attack"
            },
        };
        return data;
    }

    public void OnReset()
    {
        ClassManager.Instance.Free(behaviorTree);
    }
}