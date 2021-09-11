/**
 * 组件基类
 * @author ligzh
 * @date 2021/9/1
 */


public interface IComponent
{
    void OnInitialize(UnitBase unit);

    void OnBirth();

    void OnDead();

    void OnUpdate();
}