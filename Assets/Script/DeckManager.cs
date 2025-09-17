using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    //控制逻辑层的牌库和手牌
    public static DeckManager Instance { get; private set; }

    [SerializeField]
    public List<Card> deck  = new List<Card>(); // 牌库数组，可以根据需要调整大小

    [SerializeField]
    public List<Card> hand = new List<Card>();

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
}
