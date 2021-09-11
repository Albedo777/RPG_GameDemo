/**
 * 基于组件的战斗单位
 * @author ligzh
 * @date 2021/9/3
 */

public class BattleUnit : UnitBase
{
    public override void Initialize(UnitData data)
    {
        base.Initialize(data);
        data.battleUnit = this;
        InitComponents();
        OnInitialize();
    }

    protected virtual void InitComponents()
    {
        //所有单位都需要渲染组件
        AddComponent<RenderComponent>();

        //主角需要输入控制组件和技能释放组件
        if (Data.id == 1)
        {
            AddComponent<MoveComponent>();
            AddComponent<SkillComponent>();
        }

        //敌人需要AI行为组件和血条显示组件
        if (Data.id > 1)
        {
            AddComponent<BehaviorComponent>();
            AddComponent<HudComponent>();
        }
    }
}