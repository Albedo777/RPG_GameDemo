using UnityEngine;
using UnityEngine.UI;

/**
 * 顶部UI
 * 显示玩家的血量
 * @author ligzh
 * @date 2021/9/4
 */

public class TopUI : UIBase
{
    private Slider hpSlider;
    private Text hpText;
    private float totalHp;
    private float currentHp;

    private void Awake()
    {
        hpSlider = GetComponentInChildren<Slider>();
        hpText = transform.Find("Root/HpTextBG/Text").GetComponent<Text>();
        GetComponentInChildren<Button>().onClick.AddListener(() => UiManager.Instance.OpenUI("PauseUI"));
    }

    public override void OnOpen()
    {
        base.OnOpen();
        totalHp = UnitManager.Instance.GetPlayerUnit().Data.Hp;
        currentHp = totalHp;
        hpSlider.value = currentHp / totalHp;
        hpText.text = currentHp.ToString();
        UiManager.Instance.AddListener("ChangeHpDisplay", ChangeHpDisplay);
    }

    public override void OnClose()
    {
        base.OnClose();
        UiManager.Instance.RemoveListener("ChangeHpDisplay");
    }

    private void ChangeHpDisplay(float hpChangeVal)
    {
        currentHp += hpChangeVal;
        currentHp = Mathf.Clamp(currentHp, 0, totalHp);
        hpSlider.value = currentHp / totalHp;
        hpText.text = Mathf.Floor(currentHp).ToString();
    }
}