using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * IClass类管理者
 * @author ligzh
 * @date 2021/7/3
 */

public class ClassManager
{
    private static ClassManager instance;

    public static ClassManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ClassManager();
            }

            return instance;
        }
    }

    private Dictionary<int, ClassPool> poolDict = new Dictionary<int, ClassPool>();
    
    public void PreStore(Type type, int count)
    {
        int index = type.MetadataToken;
        if (poolDict.ContainsKey(index))
        {
            poolDict[index].PreStore(count);
        }
        else
        {
            ClassPool pool = new ClassPool(type, count);
            poolDict.Add(index, pool);
        }
    }

    public void Store<T>(int count)
    {
        Type type = typeof(T);
        PreStore(type, count);
    }

    public IClass Get(string name)
    {
        bool has = false;
        int index = 0;
        IClass instance = null;
        foreach (var item in poolDict)
        {
            if (name == item.Value.typeName)
            {
                has = true;
                index = item.Key;
            }
        }

        if (has)
        {
            instance = poolDict[index].Get();
        }
        else
        {
            Type type = Type.GetType(name);
            index = type.MetadataToken;
            poolDict.Add(index, new ClassPool(type, 1));
            instance = poolDict[index].Get();
        }

        return instance;
    }

    public T Get<T>() where T : IClass
    {
        Type type = typeof(T);
        IClass instance;
        int index = type.MetadataToken;
        ClassPool pool;
        if (poolDict.TryGetValue(index, out pool))
        {
            instance = pool.Get();
        }
        else
        {
            poolDict.Add(index, new ClassPool(type, 1));
            instance = poolDict[index].Get();
        }

        return (T) instance;
    }

    public void Free(IClass obj)
    {
        if (obj == null)
        {
            return;
        }

        int index = obj.GetType().MetadataToken;
        ClassPool pool;
        if (poolDict.TryGetValue(index, out pool))
        {
            pool.Free(obj);
        }
        else
        {
            Debug.Log("FreeObject dont has this type:" + obj.GetType());
        }
    }
}