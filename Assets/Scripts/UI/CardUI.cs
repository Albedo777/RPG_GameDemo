using UnityEngine;
using UnityEngine.UI;

/**
 * 卡牌UI界面
 * 开牌的费用条和消耗文本显示
 * @author ligzh
 * @date 2021/9/4
 */

public class CardUI : UIBase
{
    private Slider costSlider;
    private Text costText;
    private float totalCost = 10f;
    private float currentCost = 10f;

    private void Awake()
    {
        costSlider = GetComponentInChildren<Slider>();
        costText = transform.Find("Root/CostTextBG/Text").GetComponent<Text>();
        costSlider.value = currentCost / totalCost;
        costText.text = currentCost.ToString();
    }

    public override void OnOpen()
    {
        base.OnOpen();
        UiManager.Instance.AddListener("ChangeCost", ChangeCost);
    }

    public override void OnClose()
    {
        base.OnClose();
        UiManager.Instance.RemoveListener("ChangeCost");
    }

    private void ChangeCost(float costChangeVal)
    {
        currentCost += costChangeVal;
        currentCost = Mathf.Clamp(currentCost, 0, totalCost);
        costSlider.value = currentCost / totalCost;
        costText.text = Mathf.Floor(currentCost).ToString();
    }
}