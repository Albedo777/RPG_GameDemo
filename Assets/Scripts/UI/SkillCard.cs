using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
 * 卡牌UI
 * 卡牌的拖动效果
 * 技能的释放
 * @author ligzh
 * @date 2021/9/4
 */

public class SkillCard : UIBase, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public BattleUnit unit;
    private Animation animation;
    private float skillReleaseHeight = 50f;
    private Vector3 startPosition;
    public SkillCardConfig config;
    private Image iconImg;
    private Text costText;

    private void Awake()
    {
        CardManager.Instacne.AddCard(this);
        iconImg = transform.Find("Root/Bg/mask/imgCard").GetComponent<Image>();
        costText = transform.Find("Root/Bg/imgCost/txtCost").GetComponent<Text>();
    }

    public void InitCard()
    {
        iconImg.sprite = ResourceManager.Instance.LoadSpriteResource("SKillItems/" + config.iconName);
        costText.text = config.cost.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        if (transform.position.y > skillReleaseHeight)
        {
            GetComponent<CanvasGroup>().alpha = 0;
            unit.Transmit(ETransmitType.ShowDirector);
            var arg = new DirectorMoveArg();
            arg.position = GetGroundPositionByScreenPosition(eventData.position);
            unit.Transmit(ETransmitType.DirectorMove, arg);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPosition;

        if (unit == null)
        {
            return;
        }

        if (transform.position.y > skillReleaseHeight)
        {
            SkillCastArg arg = new SkillCastArg();
            arg.modelName = config.modelName;
            arg.cost = config.cost;
            unit.Transmit(ETransmitType.SkillRelase, arg);
        }

        GetComponent<CanvasGroup>().alpha = 1;
        unit.Transmit(ETransmitType.HideDirector);
    }

    private Vector3 GetGroundPositionByScreenPosition(Vector2 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(pos.x, pos.y, 0f));
        var origin = ray.origin;
        var direction = ray.direction;
        var p = origin.y / direction.y;
        return origin - direction * p;
    }
}