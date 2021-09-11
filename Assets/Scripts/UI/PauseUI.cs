using UnityEngine;
using UnityEngine.UI;

/**
 * 暂停UI界面
 * @author ligzh
 * @date 2021/9/6
 */

public class PauseUI : UIBase
{
    private void Awake()
    {
        GetComponentInChildren<Button>().onClick.AddListener(()=> UiManager.Instance.CloseUI("PauseUI"));
    }

    public override void OnOpen()
    {
        base.OnOpen();
        TimeManager.GlobalTimeScale = 0;
        TimeManager.GameTimeScale = 0;
        Time.timeScale = 0;
    }

    public override void OnClose()
    {
        TimeManager.GlobalTimeScale = 1;
        TimeManager.GameTimeScale = 1;
        Time.timeScale = 1;
        base.OnClose();
    }
}
