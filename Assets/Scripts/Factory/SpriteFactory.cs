using System.Collections.Generic;
using UnityEngine;

/**
 * 精灵图片工厂接口
 * @author ligzh
 * @date 2021/6/12
 */

public class SpriteFactory : IBaseResourceFactory<Sprite>
{
    protected Dictionary<string, Sprite> factoryDict = new Dictionary<string, Sprite>();
    protected string loadPath;

    public SpriteFactory()
    {
        loadPath = "Sprites/";
    }

    public Sprite GetSingleResource(string resourcePath)
    {
        Sprite itemGo = null;
        string itemLoadPath = loadPath + resourcePath;
        if (factoryDict.ContainsKey(resourcePath))
        {
            itemGo = factoryDict[resourcePath];
        }
        else
        {
            itemGo = Resources.Load<Sprite>(itemLoadPath);
            factoryDict.Add(resourcePath, itemGo);
        }

        if (itemGo == null)
        {
            Debug.Log("路径" + resourcePath + "资源获取失败");
        }

        return itemGo;
    }
}