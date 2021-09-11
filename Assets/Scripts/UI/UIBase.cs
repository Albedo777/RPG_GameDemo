using UnityEngine;

/**
 * UI基类
 * UI的生命周期
 * @author ligzh
 * @date 2021/9/4
 */

public class UIBase : MonoBehaviour, IClass
{

    public virtual void OnOpen()
    {
        
    }

    public virtual void OnClose()
    {
        
    }

    public void OnReset()
    {
        
    }
}

public class UIInfo
{
    public string name;
    public UILyaer layer;
    public UIType type;
}
