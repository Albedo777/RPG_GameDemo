using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 资源管理者
 * 加载并实例化预制体资源
 * 加载图片音频资源
 * 回收资源
 * @author ligzh
 * @date 2021/9/6
 */

public class ResourceManager : IClass
{
    private static ResourceManager instance;

    public static ResourceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = ClassManager.Instance.Get<ResourceManager>();
            }

            return instance;
        }
    }

    private Dictionary<FactoryType, IBaseFactory> factoryDict = new Dictionary<FactoryType, IBaseFactory>();
    private AudioClipFactory audioClipFactory;
    private SpriteFactory spriteFactory;
    
    public void Init()
    {
        factoryDict.Add(FactoryType.UIFactory, new UIFactory());
        factoryDict.Add(FactoryType.GameFactory, new GameFactory());
        audioClipFactory = new AudioClipFactory();
        spriteFactory = new SpriteFactory();
    }

    public void PreLoadGameObjectResource(FactoryType type, string resourcePath, int num)
    {
        GameObject[] temp = new GameObject[num];
        for (int i = 0; i < num; i++)
        {
            temp[i] = factoryDict[type].GetItem(resourcePath);
        }

        for (int i = 0; i < num; i++)
        {
            factoryDict[type].PushItem(resourcePath, temp[i]);
        }
    }

    public GameObject LoadGameObjectResource(FactoryType type, string resourcePath, Action onLoadCompleted = null)
    {
        GameObject go = factoryDict[type].GetItem(resourcePath);
        if (onLoadCompleted != null)
        {
            onLoadCompleted();
        }

        return go;
    }

    public AudioClip LoadAudioResource(string resourcePath)
    {
        return audioClipFactory.GetSingleResource(resourcePath);
    }

    public Sprite LoadSpriteResource(string resourcePath)
    {
        return spriteFactory.GetSingleResource(resourcePath);
    }

    public void FreeResource(FactoryType type, string resourcePath, GameObject item)
    {
        factoryDict[type].PushItem(resourcePath, item);
    }

    public void OnReset()
    {
        factoryDict.Clear();
        factoryDict = null;
        spriteFactory = null;
        audioClipFactory = null;
        ClassManager.Instance.Free(instance);
    }
    
}