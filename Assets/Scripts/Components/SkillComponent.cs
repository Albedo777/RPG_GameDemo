using System.Collections.Generic;
using UnityEngine;

/**
 * 技能释放组件
 * @author ligzh
 * @date 2021/9/4
 */

public class SkillComponent : IComponent, ITransmit
{
    private UnitBase unit;
    private GameObject director;
    private Vector3 targetPosition;
    private Vector3 targetForward;
    private List<UnitBase> targetList = new List<UnitBase>();

    public void OnInitialize(UnitBase unit)
    {
        this.unit = unit;
    }

    public void OnBirth()
    {
    }

    public void OnDead()
    {
    }

    public void OnUpdate()
    {
    }

    public void OnTransmit(ETransmitType type, BaseTransmitArg arg)
    {
        if (type == ETransmitType.SkillRelase)
        {
            SkillCastArg skillArg = arg as SkillCastArg;
            //技能实例化
            GameObject skill =
                ResourceManager.Instance.LoadGameObjectResource(FactoryType.GameFactory, skillArg.modelName);
            skill.transform.position = unit.Data.unitTrans.position;
            skill.transform.SetParent(GameManager.Instance.transform);
            skill.transform.forward = targetForward;
            unit.Data.unitTrans.forward = targetForward;
            //得到敌人单位
            UnitBase enemyUnit = GetClosetEnemy();
            //敌人扣血
            HpChangeArg hpArg = ClassManager.Instance.Get<HpChangeArg>();
            hpArg.changHpVal = -unit.Data.attack;
            enemyUnit.Transmit(ETransmitType.ChangeHp, hpArg);
            ClassManager.Instance.Free(hpArg);
            //敌人播放被击中动画
            enemyUnit.Transmit(ETransmitType.BeHit);
            //更改费用显示
            UiManager.Instance.DispatchEvent("ChangeCost", -skillArg.cost);
            //技能资源回收
            TimeManager.Instance.DelayCall(4000,
                () => { ResourceManager.Instance.FreeResource(FactoryType.GameFactory, skillArg.modelName, skill); });
        }

        if (type == ETransmitType.ShowDirector)
        {
            GetDirector();
        }

        if (type == ETransmitType.HideDirector)
        {
            if (director != null)
            {
                director.SetActive(false);
            }
        }

        if (type == ETransmitType.DirectorMove)
        {
            if (director != null)
            {
                var moveArg = arg as DirectorMoveArg;
                MoveDirector(moveArg.position, director);
            }
        }
    }

    private void GetDirector()
    {
        if (director == null)
        {
            director = ResourceManager.Instance.LoadGameObjectResource(FactoryType.GameFactory,
                "Director/sfx_director_rect");
        }

        if (!director.activeSelf)
        {
            director.SetActive(true);
        }

        director.transform.SetParent(unit.Data.unitTrans);
        director.transform.forward = Vector3.forward;
    }

    private UnitBase GetClosetEnemy()
    {
        float minDis = int.MaxValue;
        UnitBase closetEnemy = null;
        List<UnitBase> enemyList = UnitManager.Instance.GetEnemyUnitList();
        foreach (var enemy in enemyList)
        {
            float distance = Mathf.Abs(Vector3.Distance(unit.Data.unitTrans.position, enemy.Data.unitTrans.position));
            if (distance < minDis)
            {
                minDis = distance;
                closetEnemy = enemy;
            }
        }

        return closetEnemy;
    }

    private void MoveDirector(Vector3 position, GameObject director)
    {
        Vector3 dir = position - unit.Data.unitTrans.position;
        dir.y = 0;
        targetPosition = position;
        targetForward = dir.normalized;
        targetPosition.y = 0;
        director.transform.position = unit.Data.unitTrans.position + targetForward * 0.1f;
        if (targetForward != Vector3.zero)
        {
            director.transform.forward = targetForward;
        }
    }
}