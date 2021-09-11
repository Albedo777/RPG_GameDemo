using System.Collections.Generic;
using UnityEngine;

/**
 * 动画控制器工厂接口
 * @author ligzh
 * @date 2021/6/12
 */

public class RuntimeAniamtiorControllerFactory : IBaseResourceFactory<RuntimeAnimatorController>
{
    protected Dictionary<string, RuntimeAnimatorController> factoryDict =
        new Dictionary<string, RuntimeAnimatorController>();

    protected string loadPath;

    public RuntimeAniamtiorControllerFactory()
    {
        loadPath = "Animator/AnimatorController/";
    }

    public RuntimeAnimatorController GetSingleResource(string resourcePath)
    {
        RuntimeAnimatorController itemGo = null;
        string itemLoadPath = loadPath + resourcePath;
        if (factoryDict.ContainsKey(resourcePath))
        {
            itemGo = factoryDict[resourcePath];
        }
        else
        {
            itemGo = Resources.Load<RuntimeAnimatorController>(itemLoadPath);
            factoryDict.Add(resourcePath, itemGo);
        }

        if (itemGo == null)
        {
            Debug.Log("路径" + resourcePath + "资源获取失败");
        }

        return itemGo;
    }
}