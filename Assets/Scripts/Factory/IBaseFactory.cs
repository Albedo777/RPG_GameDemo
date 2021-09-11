using UnityEngine;

/**
 * 游戏物体工厂接口
 * @author ligzh
 * @date 2021/6/12
 */

public interface IBaseFactory
{
    GameObject GetItem(string itemName);
    void PushItem(string itemName, GameObject item);
}
