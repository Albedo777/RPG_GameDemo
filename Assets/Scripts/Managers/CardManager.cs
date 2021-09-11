using System.Collections.Generic;
using UnityEngine;

/**
 * 牌组管理者
 * @author ligzh
 * @date 2021/9/5
 */

public class CardManager : IClass
{
    public BattleUnit unit;
    private List<SkillCard> hand = new List<SkillCard>();

    private static CardManager instacne;

    public static CardManager Instacne
    {
        get
        {
            if (instacne == null)
            {
                instacne = ClassManager.Instance.Get<CardManager>();
            }

            return instacne;
        }
    }

    public void Init(BattleUnit unit, List<SkillCardConfig> configs)
    {
        if (hand.Count != configs.Count)
        {
            Debug.LogError("配置文件与卡牌数量不符，初始化失败");
            return;
        }

        this.unit = unit;
        for (int i = 0; i < configs.Count; i++)
        {
            hand[i].unit = unit;
            hand[i].config = configs[i];
            hand[i].InitCard();
        }
    }

    public void AddCard(SkillCard card)
    {
        hand.Add(card);
    }

    public void OnReset()
    {
        ClassManager.Instance.Free(instacne);
    }
}