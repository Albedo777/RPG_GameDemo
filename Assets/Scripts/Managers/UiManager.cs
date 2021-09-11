using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * UI管理者
 * UI的层级顺序管理
 * UI的打开关闭，事件注册发送
 * @author ligzh
 * @date 2021/9/6
 */

public class UiManager : IClass
{
    private static UiManager instance;
    private GameObject topCanvas;
    private GameObject bottomCanvas;
    private GameObject maskCanvas;
    private GameObject windowCanvas;

    private Stack<GameObject> stack;
    private Dictionary<string, UIBase> scriptsDict;
    private Dictionary<string, UIInfo> configDict;
    private Dictionary<string, Action<float>> registerMethodDict;
    private GameObject currentUI;

    public static UiManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = ClassManager.Instance.Get<UiManager>();
            }

            return instance;
        }
    }

    public void Init()
    {
        stack = new Stack<GameObject>();
        scriptsDict = new Dictionary<string, UIBase>();
        configDict = new Dictionary<string, UIInfo>();
        registerMethodDict = new Dictionary<string, Action<float>>();
        topCanvas = GameObject.Find("TopCanvas");
        bottomCanvas = GameObject.Find("BottomCanvas");
        maskCanvas = GameObject.Find("MaskCanvas");
        windowCanvas = GameObject.Find("WindowCanvas");
    }

    public void Update()
    {
        DispatchEvent("ChangeCost", Time.deltaTime);
    }


    public void LoadUIConfig(UIInfo data)
    {
        configDict.Add(data.name, data);
    }

    public void OpenUI(string name)
    {
        UIInfo config = configDict[name];
        GameObject go = ResourceManager.Instance.LoadGameObjectResource(FactoryType.UIFactory, config.name);
        if (!scriptsDict.ContainsKey(name))
        {
            scriptsDict.Add(name, go.GetComponent<UIBase>());
        }

        scriptsDict[name].OnOpen();
        switch (config.layer)
        {
            case UILyaer.Bottom:
                go.transform.SetParent(bottomCanvas.transform);
                break;
            case UILyaer.Top:
                go.transform.SetParent(topCanvas.transform);
                break;
            case UILyaer.Window:
                go.transform.SetParent(windowCanvas.transform);
                break;
            case UILyaer.Mask:
                go.transform.SetParent(maskCanvas.transform);
                break;
        }

        go.transform.localPosition = Vector3.zero;
        go.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        switch (config.type)
        {
            case UIType.Freedom:
                break;
            case UIType.Order:
                if (currentUI != null)
                {
                    CloseUI(currentUI.name);
                }

                currentUI = go;
                break;
            case UIType.Stack:
                stack.Push(go);
                break;
        }
    }

    public void CloseUI(string name)
    {
        GameObject go = scriptsDict[name].gameObject;
        UIInfo config = configDict[name];
        scriptsDict[name].OnClose();
        switch (config.type)
        {
            case UIType.Freedom:
                ResourceManager.Instance.FreeResource(FactoryType.UIFactory, name, go);
                break;
            case UIType.Order:
                ResourceManager.Instance.FreeResource(FactoryType.UIFactory, name, go);
                currentUI = null;
                break;
            case UIType.Stack:
                ResourceManager.Instance.FreeResource(FactoryType.UIFactory, name, go);
                stack.Pop();
                break;
        }
    }

    public void AddListener(string name, Action<float> action)
    {
        registerMethodDict.Add(name, action);
    }

    public void DispatchEvent(string name, float val = 0)
    {
        foreach (var method in registerMethodDict)
        {
            if (method.Key == name && method.Value != null)
            {
                method.Value(val);
            }
        }
    }

    public void RemoveListener(string name)
    {
        registerMethodDict.Remove(name);
    }

    public void OnReset()
    {
        ClassManager.Instance.Free(instance);
    }
}


public enum UILyaer
{
    Top = 0,
    Bottom = 1,
    Window = 2,
    Mask = 3,
}

public enum UIType
{
    //自由控制类型界面（不做任何约束）
    Freedom,

    //stackInCount: 栈中默认只保留3个，多处3个则释放界面，但栈信息还在，回栈会重新创建界面
    //stackOutCount: 出了栈之后的界面只保留2个作为缓存
    Stack,

    //顺序显示模式界面, orderOutCount 唯一队列模式的界面只缓存1个
    Order,
}