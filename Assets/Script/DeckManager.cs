using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    //控制逻辑层的牌库和手牌
    public static DeckManager Instance { get; private set; }

    [SerializeField]
    public List<Card> deck  = new List<Card>(); // 牌库数组，可以根据需要调整大小

    [SerializeField]
    public List<Card> hand = new List<Card>();

    public List<int> extraCertainCardId = null;//额外抽取特定id的牌
    public List<CardType> extraCertainCardType = null;//额外抽取特定种类的牌
    public List<CardType> reduceCertainCardType = null;//减少抽取特定种类的牌

    private int extraDrawNum = 0;
    public int ExtraDrawNum {
        get { return extraDrawNum; }
        set { extraDrawNum = value; }
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    // 其他方法和属性可以在这里添加

    public void InitializingData() {
        deck.Clear();
        hand.Clear();
    }

    public void ShuffleDeck() {
        // 实现洗牌逻辑
        List<Card> shuffledDeck = new List<Card>(deck);
        for (int i = shuffledDeck.Count - 1;i > 0;i--) {
            int j = Random.Range(0,i + 1);
            // 交换元素
            Card temp = shuffledDeck[i];
            shuffledDeck[i] = shuffledDeck[j];
            shuffledDeck[j] = temp;
        }
        deck = shuffledDeck; // 更新牌库为洗好的牌
        Debug.Log("已洗牌");
    }

    public void DrawCard() {
        // 实现抽卡逻辑
        if (deck.Count == 0) {
            Debug.LogWarning("牌库空，无法抓牌");
            return;
        }
        Card drawnCard = deck[0]; // 假设抽取第一张卡
        deck.RemoveAt(0); // 从牌库中移除抽取的卡片
        hand.Add(drawnCard); // 将抽取的卡片添加到手牌中
        Debug.Log("抓了一张牌");
    }

    public void DrawCertainCardByType(CardType cardType) {
        //抽取特定种类的牌
        for (int i = 0;i < deck.Count;i++) {
            if (deck[i].CardType == cardType) {
                Card drawnCard = deck[i];
                deck.RemoveAt(i);
                hand.Add(drawnCard);
                Debug.Log($"抓了一张{cardType}牌：{drawnCard.Name}");
                ShuffleDeck();
                return;
            }
        }
        Debug.LogWarning($"牌库中没有{cardType}牌，无法抓牌");
    }

    public void DrawCertainCardById(int id) {
        //抽取特定id的牌
        for (int i = 0;i < deck.Count;i++) {
            if (deck[i].Id == id) {
                Card drawnCard = deck[i];
                deck.RemoveAt(i);
                hand.Add(drawnCard);
                Debug.Log($"抓了一张id为{id}的牌：{drawnCard.Name}");
                ShuffleDeck();
                return;
            }
        }
        Debug.LogWarning($"牌库中没有id为{id}的牌，无法抓牌");
    }

    public bool RemoveCertainCardByType(CardType cardType) {
        for (int i = 0;i < deck.Count;i++) {
            if (deck[i].CardType == cardType) {
                Card removedCard = deck[i];
                deck.RemoveAt(i);
                Debug.Log($"移除了牌库中的一张{cardType}牌：{removedCard.Name}");
                return true;
            }
        }
        Debug.LogWarning($"牌库中没有{cardType}牌，无法移除");
        return false;
    }

    public bool RemoveCertainCardById(int id) {
        for (int i = 0;i < deck.Count;i++) {
            if (deck[i].Id == id) {
                Card removedCard = deck[i];
                deck.RemoveAt(i);
                Debug.Log($"移除了牌库中的一张id为{id}的牌：{removedCard.Name}");
                return true;
            }
        }
        Debug.LogWarning($"牌库中没有id为{id}的牌，无法移除");
        return false;
    }

    public int CountCertainCardById(int id) {
        int count = 0;
        for (int i = 0;i < deck.Count;i++) {
            if (deck[i].Id == id) {
                count++;
            }
        }
        return count;
    }

    public bool TryToRemoveRemoveCertainCardByIdMultipleTime(int id,int num) {
        int count = CountCertainCardById(id);
        if (count >= num) {
            for (int i = 0;i < num;i++) {
                RemoveCertainCardById(id);
            }
            return true;
        } else {
            Debug.LogWarning($"牌库中没有足够的id为{id}的牌，无法移除");
            return false;
        }
    }

    public void AddCardToDeck(Card card) {
        // 实现将卡片添加到牌库的逻辑
        deck.Add(card);
        Debug.Log($"卡牌 {card.Name} 加入了牌库");
    }

    public void AddCardToDeck(Card card,int num) {
        for (int i = 0;i < num;i++) {
            deck.Add(card);
        }
        Debug.Log($"卡牌 {card.Name} 加入了牌库 {num} 张");
    }

    public void ClearDeck() {
        // 实现清空牌库的逻辑
        Debug.Log("牌库已清空");
    }

    public void ClearHand() {
        // 实现清空手牌的逻辑
        hand.Clear();
        Debug.Log("手牌已清空");
    }


    public void AddCardToDeckFromLand(Card card,int num,Land land) {
        //留给地块调用
        //这里可以加一些动画之类的
        AddCardToDeck(card,num);
    }


    public void DrawPhase() {
        // 抽卡阶段逻辑
        Debug.Log("抽卡阶段");
        // 这里可以添加更多的逻辑，比如更新UI，处理状态等
        ShuffleDeck();
        if (reduceCertainCardType != null) {
            if (reduceCertainCardType.Contains(CardType.MATERIAL)) {
                for (int i = 0;i < 4 - reduceCertainCardType.Count(x => x == CardType.MATERIAL); i++) {
                    DrawCertainCardByType(CardType.MATERIAL);
                }
            } else if (reduceCertainCardType.Contains(CardType.SKILL)) {
                for (int i = 0;i < 3 - reduceCertainCardType.Count(x => x == CardType.SKILL);i++) {
                    DrawCertainCardByType(CardType.SKILL);
                }
            }
            reduceCertainCardType.Clear();
            return;
        } else {
            for (int i = 0;i < 4;i++) {
                DrawCertainCardByType(CardType.MATERIAL);
            }
            for (int i = 0;i < 3;i++) {
                DrawCertainCardByType(CardType.SKILL);
            }
        }
    }

    public void ExtraDrawPhase() {
        //先抽特定ID的
        for (int i = 0;i < extraCertainCardId.Count;i++) {
            DrawCertainCardById(extraCertainCardId[i]);
        }
        extraCertainCardId.Clear();
        //再抽特定种类的
        for (int i = 0;i < extraCertainCardType.Count;i++) {
            DrawCertainCardByType(extraCertainCardType[i]);
        }
        extraCertainCardType.Clear();
        //再抽额外的
        for (int i = 0;i < ExtraDrawNum;i++) {
            DrawCard();
        }
    }


    public void DisasterPhase() {
        //回合结束时，销毁手中剩余的资源卡和技能卡，如果剩余了天灾卡则触发天灾
        Debug.Log("天灾阶段");
        foreach (Card card in hand) {
            if (card.CardType == CardType.DISASTER) {
                //触发天灾效果
                Debug.Log($"触发了天灾卡：{card.Name}");
                card.ApplyEffect(null); // 这里传入null，表示没有特定的地块
            }
        }
    }
}
