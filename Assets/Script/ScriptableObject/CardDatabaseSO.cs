using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌数据库ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "CardDatabaseSO",menuName = "Card/CardDatabaseSO")]
public class CardDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public class CardEntry
    {
        public int cardId;
        public CardScriptableObject cardSO;
    }


    [SerializeField]
    private List<CardEntry> materialCardEntries = new List<CardEntry>();
    [SerializeField]
    private List<CardEntry> skillCardEntries = new List<CardEntry>();
    [SerializeField]
    private List<CardEntry> weaponCardEntries = new List<CardEntry>();
    [SerializeField]
    private List<CardEntry> disasterCardEntries = new List<CardEntry>();


    [SerializeField]
    private Dictionary<int,CardScriptableObject> cardDict;


    public void InitDictionary() {
        cardDict = new Dictionary<int,CardScriptableObject>();

        // 遍历材料卡牌列表
        foreach (var entry in materialCardEntries) {
            if (entry != null && entry.cardSO != null && !cardDict.ContainsKey(entry.cardId)) {
                cardDict.Add(entry.cardId,entry.cardSO);
            }
        }

        // 遍历技能卡牌列表
        foreach (var entry in skillCardEntries) {
            if (entry != null && entry.cardSO != null && !cardDict.ContainsKey(entry.cardId)) {
                cardDict.Add(entry.cardId,entry.cardSO);
            }
        }

        // 遍历武器卡牌列表
        foreach (var entry in weaponCardEntries) {
            if (entry != null && entry.cardSO != null && !cardDict.ContainsKey(entry.cardId)) {
                cardDict.Add(entry.cardId,entry.cardSO);
            }
        }

        // 遍历灾难卡牌列表
        foreach (var entry in disasterCardEntries) {
            if (entry != null && entry.cardSO != null && !cardDict.ContainsKey(entry.cardId)) {
                cardDict.Add(entry.cardId,entry.cardSO);
            }
        }

        Debug.Log($"CardDatabaseSO字典初始化完成，共加载 {cardDict.Count} 张卡牌");
    }

    public CardScriptableObject GetCardSO(int cardId) {
        if (cardDict == null) {
            Debug.LogWarning("Card dictionary未正确初始化");
            return null;
        }
        if (cardDict.TryGetValue(cardId,out var cardSO)) {
            return cardSO;
        } else {
            Debug.LogWarning($"无法找到卡牌ID {cardId} 的 ScriptableObject。字典中共有 {cardDict.Count} 张卡牌");
            return null;
        }
    }
}

