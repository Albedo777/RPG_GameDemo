using System.Collections.Generic;
using UnityEngine;

/**
 * AudioClip 工厂
 * @author ligzh
 * @date 2021/6/12
 */

public class AudioClipFactory : IBaseResourceFactory<AudioClip>
{
    protected Dictionary<string, AudioClip> factoryDict = new Dictionary<string, AudioClip>();
    protected string loadPath;

    public AudioClipFactory()
    {
        loadPath = "AudioClips/";
    }

    public AudioClip GetSingleResource(string resourcePath)
    {
        AudioClip itemGo = null;
        string itemLoadPath = loadPath + resourcePath;
        if (factoryDict.ContainsKey(resourcePath))
        {
            itemGo = factoryDict[resourcePath];
        }
        else
        {
            itemGo = Resources.Load<AudioClip>(itemLoadPath);
            factoryDict.Add(resourcePath, itemGo);
        }

        if (itemGo == null)
        {
            Debug.Log("路径" + resourcePath + "资源获取失败");
        }

        return itemGo;
    }
}