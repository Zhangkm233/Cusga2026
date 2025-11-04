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

    public void TestAddCard() {

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

    public void DrawCertainCardByTypes(params CardType[] cardTypes) {
        //抽取特定种类们的牌
        for (int i = 0;i < deck.Count;i++) {
            if (cardTypes.Contains<CardType>(deck[i].CardType)) {
                Card drawnCard = deck[i];
                deck.RemoveAt(i);
                hand.Add(drawnCard);
                Debug.Log($"抓了一张{drawnCard.CardType}牌：{drawnCard.CardName}");
                ShuffleDeck();
                return;
            }
        }
        foreach (CardType type in cardTypes) {
            Debug.LogWarning($"牌库中没有{type}牌，无法抓牌");
        }
    }

    public void DrawCertainCardByType(CardType cardType) {
        //抽取特定种类的牌
        for (int i = 0;i < deck.Count;i++) {
            if (deck[i].CardType == cardType) {
                Card drawnCard = deck[i];
                deck.RemoveAt(i);
                hand.Add(drawnCard);
                Debug.Log($"抓了一张{cardType}牌：{drawnCard.CardName}");
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
                Debug.Log($"抓了一张id为{id}的牌：{drawnCard.CardName}");
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
                Debug.Log($"移除了牌库中的一张{cardType}牌：{removedCard.CardName}");
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
                Debug.Log($"移除了牌库中的一张id为{id}的牌：{removedCard.CardName}");
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
    public int CountCertainCardByType(CardType cardType) {
        int count = 0;
        for (int i = 0;i < deck.Count;i++) {
            if (deck[i].CardType == cardType) {
                count++;
            }
        }
        return count;
    }

    public bool TryToRemoveCertainCardByIdMultipleTime(int id,int num) {
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
    public bool TryToRemoveCertainCardByTypeMultipleTime(CardType cardType,int num) {
        int count = CountCertainCardByType(cardType);
        if (count >= num) {
            for (int i = 0;i < num;i++) {
                RemoveCertainCardByType(cardType);
            }
            return true;
        } else {
            Debug.LogWarning($"牌库中没有足够的{cardType}的牌，无法移除");
            return false;
        }
    }


    public void AddCardToDeck(Card card) {
        // 实现将卡片添加到牌库的逻辑
        deck.Add(card);
        Debug.Log($"卡牌 {card.CardName} 加入了牌库");
    }

    public void AddCardToDeck(Card card,int num) {
        for (int i = 0;i < num;i++) {
            deck.Add(card);
        }
        Debug.Log($"卡牌 {card.CardName} 加入了牌库 {num} 张");
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
        //抽卡阶段-抽取4张资源卡，3张技能卡
        ShuffleDeck();
        if (reduceCertainCardType != null) {
            Debug.Log($"减少抽取{reduceCertainCardType.Count}张特定种类的牌");
            if (reduceCertainCardType.Contains(CardType.MATERIAL)) {
                for (int i = 0;i < 4 - reduceCertainCardType.Count(x => x == CardType.MATERIAL);i++) {
                    DrawCertainCardByTypes(CardType.MATERIAL,CardType.DISASTER);
                }
            } else {
                for (int i = 0;i < 4;i++) {
                    DrawCertainCardByTypes(CardType.MATERIAL,CardType.DISASTER);
                }
            }
            if (reduceCertainCardType.Contains(CardType.SKILL)) {
                for (int i = 0;i < 3 - reduceCertainCardType.Count(x => x == CardType.SKILL);i++) {
                    DrawCertainCardByTypes(CardType.SKILL,CardType.WEAPON);
                }
            } else {
                for (int i = 0;i < 3;i++) {
                    DrawCertainCardByTypes(CardType.SKILL,CardType.WEAPON);
                }
            }
            reduceCertainCardType.Clear();
            return;
        } else {
            Debug.Log("正常抽卡");
            for (int i = 0;i < 4;i++) {
                DrawCertainCardByTypes(CardType.MATERIAL,CardType.DISASTER);
            }
            for (int i = 0;i < 3;i++) {
                DrawCertainCardByTypes(CardType.SKILL,CardType.WEAPON);
            }
        }
        Debug.Log($"手牌数量：{hand.Count}张");
    }

    public void ExtraDrawPhase() {
        //先抽特定ID的
        Debug.Log($"额外抽取{extraCertainCardId.Count}张特定ID的牌");
        for (int i = 0;i < extraCertainCardId.Count;i++) {
            Debug.Log($"额外抽取特定ID的牌：{extraCertainCardId[i]}");
            DrawCertainCardById(extraCertainCardId[i]);
        }
        extraCertainCardId.Clear();
        //再抽特定种类的
        Debug.Log($"额外抽取{extraCertainCardType.Count}张特定种类的牌");
        for (int i = 0;i < extraCertainCardType.Count;i++) {
            Debug.Log($"额外抽取特定种类的牌：{extraCertainCardType[i]}");
            DrawCertainCardByType(extraCertainCardType[i]);
        }
        extraCertainCardType.Clear();
        //再抽额外的 这里一般不会额外抽
        Debug.Log($"额外抽取{ExtraDrawNum}张牌");
        for (int i = 0;i < ExtraDrawNum;i++) {
            DrawCard();
        }
        ExtraDrawNum = 0;
    }


    public void DisasterPhase() {
        //回合结束时，
        //把资源卡销毁 技能卡洗回牌库 触发天灾
        Debug.Log("天灾阶段");
        foreach (Card card in hand) {
            if (card.CardType == CardType.DISASTER) {
                //触发天灾效果
                Debug.Log($"触发了天灾卡：{card.CardName}");
                card.ApplyEffect(null); // 这里传入null，表示没有特定的地块
            } else if (card.CardType == CardType.SKILL) {
                //技能卡洗回牌库
                Debug.Log($"技能卡{card.CardName}洗回牌库");
                AddCardToDeck(card);
            } else if (card.CardType == CardType.MATERIAL) {
                //资源卡销毁
                Debug.Log($"资源卡{card.CardName}被销毁");
            }
            ClearHand();
        }
    }

    public void BossAttack() {
        //Boss的攻击会将三张天灾置入你的牌库，且你下回合会额外抽到一张天灾
        for (int i = 0;i < 3;i++) {
            DisasterCard disasterCard = new DisasterCard();
            AddCardToDeck(disasterCard);
        }
        extraCertainCardType.Add(CardType.DISASTER);
    }

}
