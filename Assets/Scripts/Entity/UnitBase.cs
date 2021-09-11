using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 实体单位基类
 * 定义了战斗单位和组件的生命周期
 * @author ligzh
 * @date 2021/9/2
 */

public abstract class UnitBase : IClass
{
    private List<IComponent> m_components;
    public UnitData Data;
    public GameObject gameObject;
    public Transform rootTrans;

    public void Birth()
    {
        Data.isDead = false;
        OnBirth();
    }

    public virtual void OnBirth()
    {
        for (int i = 0; i < m_components.Count; i++)
        {
            m_components[i].OnBirth();
        }
    }

    public virtual void Initialize(UnitData data)
    {
        this.Data = data;
        gameObject = new GameObject(data.id.ToString());
        gameObject.transform.SetParent(rootTrans);
        gameObject.transform.localPosition = Vector3.zero;
        m_components = new List<IComponent>();
    }

    protected void OnInitialize()
    {
        for (int i = 0; i < m_components.Count; i++)
        {
            m_components[i].OnInitialize(this);
        }
    }

    public virtual void Update()
    {
        for (int i = 0; i < m_components.Count; i++)
        {
            m_components[i].OnUpdate();
        }
    }

    public virtual void LateUpdate()
    {
    }

    public virtual void BeforeUpdate()
    {
    }

    public virtual void Dead()
    {
        if (Data.isDead)
        {
            return;
        }

        Data.isDead = true;
        OnDead();
    }

    private void OnDead()
    {
        for (int i = 0; i < m_components.Count; i++)
        {
            m_components[i].OnDead();
        }
    }

    public void Transmit(ETransmitType type, BaseTransmitArg arg = null)
    {
        for (int i = 0; i < m_components.Count; i++)
        {
            if (m_components[i] is ITransmit)
            {
                ((ITransmit) m_components[i]).OnTransmit(type, arg);
            }
        }
    }

    protected T AddComponent<T>() where T : IComponent
    {
        var component = Activator.CreateInstance<T>();
        if (!m_components.Contains(component))
        {
            m_components.Add(component);
        }

        return component;
    }
    

    void IClass.OnReset()
    {
        if (gameObject != null)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            Debug.LogError(Data.id + " 已经被销毁");
        }

        for (int i = 0; i < m_components.Count; i++)
        {
            m_components[i] = null;
        }

        m_components.Clear();
        Data = null;
        Dispose();
    }

    protected virtual void Dispose()
    {
        
    }
}