using System;
using System.Collections.Generic;

/**
 * IClass对象池
 * @author ligzh
 * @date 2021/7/3
 */


public class ClassPool
{
    public Type type { get; set; }
    public int count { get; set; }
    public string typeName { get; set; }

    public Stack<IClass> poolStack;
    
    public ClassPool(Type type, int count)
    {
        this.type = type;
        this.count = count;
        typeName = type.Name;
        poolStack = new Stack<IClass>(count);
        for (int i = 0; i < count; i++)
        {
            var instance = Activator.CreateInstance(type) as IClass;
            poolStack.Push(instance);
        }
    }

    public void PreStore(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var instance = Activator.CreateInstance(type) as IClass;
            poolStack.Push(instance);
        }
    }

    public IClass Get()
    {
        if (poolStack.Count <= 0)
        {
            var instance = Activator.CreateInstance(type) as IClass;
            return instance;
        }

        return poolStack.Pop();
    }

    public void Free(IClass obj)
    {
        obj.OnReset();
        poolStack.Push(obj);
    }
}
