using System.Collections.Generic;
using UnityEngine;

/**
 * 游戏类型物体的工厂基类 (对象池应用)
 * @author ligzh
 * @date 2021/6/12
 */

public class BaseFactory : IBaseFactory
{
    //当前拥有的GameObject类型资源 (UI, UIPanel, Game) 存放资源 (Prefabs)
    //使用的这些资源的克隆
    protected Dictionary<string, GameObject> factoryDict = new Dictionary<string, GameObject>();

    //对象池字典，一个名字对应一种对象池
    protected Dictionary<string, Stack<GameObject>> objectPoolDict = new Dictionary<string, Stack<GameObject>>();
    //对象池(具体存储的游戏物体类型对象)，对应的是一个具体的GameObject对象

    protected string loadPath; //加载路径

    public BaseFactory()
    {
        loadPath = "Prefabs/";
    }

    //放入池子
    public void PushItem(string itemName, GameObject item)
    {
        item.transform.SetParent(GameManager.Instance.transform);
        item.SetActive(false);
        if (objectPoolDict.ContainsKey(itemName))
        {
            objectPoolDict[itemName].Push(item);
        }
        else
        {
            Debug.Log("当前字典没有" + itemName + "的栈");
        }
    }

    //取出对象
    public GameObject GetItem(string itemName)
    {
        GameObject itemGo = null;
        if (objectPoolDict.ContainsKey(itemName))
        {
            //包含此类型游戏物体的对象池
            if (objectPoolDict[itemName].Count == 0)
            {
                //该类型堆栈已经空了
                GameObject go = GetResource(itemName);
                itemGo = GameObject.Instantiate(go);
            }
            else
            {
                itemGo = objectPoolDict[itemName].Pop();
                itemGo.SetActive(true);
            }
        }
        else //不包含此类型游戏物体的对象池
        {
            //生成对象池
            objectPoolDict.Add(itemName, new Stack<GameObject>());
            //获取资源
            GameObject go = GetResource(itemName);
            //借助GameManager生成实例，对象池里存放的是实例化的游戏物体
            itemGo = GameObject.Instantiate(go);
        }

        if (itemGo == null)
        {
            Debug.Log(itemName + "的实例获取失败！");
        }

        return itemGo;
    }

    // 获取预制体资源
    private GameObject GetResource(string itemName)
    {
        GameObject itemGo = null;
        string itemPath = loadPath + itemName;
        if (factoryDict.ContainsKey(itemName))
        {
            //已经加载过资源(Prefabs)
            itemGo = factoryDict[itemName];
        }
        else
        {
            itemGo = Resources.Load<GameObject>(itemPath);
            factoryDict.Add(itemName, itemGo);
        }

        if (itemGo == null)
        {
            Debug.Log(itemName + "资源获取失败！");
            Debug.Log("失败路径" + itemPath);
        }

        return itemGo;
    }
}