using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 卡牌数据库ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "CardDatabaseSO", menuName = "Card/CardDatabaseSO")]
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


    private List<List<CardEntry>> lists;
    [SerializeField]
    private Dictionary<int, CardScriptableObject> cardDict;

    public void InitDictionary()
    {
        lists.Add(materialCardEntries);
        lists.Add(skillCardEntries);
        lists.Add(weaponCardEntries);
        lists.Add(disasterCardEntries);

        cardDict = new Dictionary<int, CardScriptableObject>();
        foreach (var list in lists)
        {
            foreach(var entry in list) {
                if (!cardDict.ContainsKey(entry.cardId))
                    cardDict.Add(entry.cardId,entry.cardSO);
            }
        }
    }

    public CardScriptableObject GetCardSO(int cardId)
    {
        if (cardDict == null) {
            Debug.LogWarning("Card dictionary未正确初始化");
            return null;
        }
        cardDict.TryGetValue(cardId, out var cardSO);
        return cardSO;
    }
}

