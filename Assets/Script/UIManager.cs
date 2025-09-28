using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // ������Ϸ�ڵ�UI��ʾ
    public GameObject[] cards;
    public TMP_Text DeckCount;
    public TMP_Text HandCount;
    public TMP_Text ExtraDrawCount;
    public static UIManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    void Start() {
        UpdateCardsSortingOrder();
    }

    [ContextMenu("UpdateCards")]
    public void UpdateCards() {
        GameObject[] cards = GameObject.FindGameObjectsWithTag("CardGameobject");
        
        for (int i = 0;i < Mathf.Min(DeckManager.Instance.hand.Count,cards.Length);i++) {
            CardController cardController = cards[i].GetComponent<CardController>();
            
            // 设置正确的索引
            cardController.indexOfCards = i;
            
            // 总是设置card数据，但只在非拖拽时更新位置
            cardController.card = DeckManager.Instance.hand[i];
            if (!cardController.isDragging) {
                cardController.UpdateCard();
            } else {
                // 拖拽时只更新文本信息，不更新位置
                if (cardController.card != null) {
                    cardController.cardName.text = cardController.card.CardName;
                    cardController.cardDescription.text = cardController.card.Description;
                    cardController.cardType.text = GameData.HanizeCardType(cardController.card.CardType);
                }
            }
        }
        
        // 隐藏多余的卡片
        for (int i = DeckManager.Instance.hand.Count; i < cards.Length; i++) {
            cards[i].SetActive(false);
        }
        
        UpdateCardsSortingOrder();
    }

    public void UpdateCardsSortingOrder() {
        if (cards == null) {
            return;
        }
        foreach (var card in cards) {
            card.GetComponent<CardController>().UpdateSortingOrder();
        }
    }
    
    /*
    public void UpdateTiles() {
        // ��ȡ����TileGameobject���󲢸������ǵ�״̬
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("TileGameobject");
        foreach (GameObject tile in tiles) {
            tile.GetComponent<TileController>().UpdateTile();
        }
    }
    */

    /*
    public void UpdateUI() {
        DeckCount.text = "�ƿ�: " + DeckManager.Instance.deck.Count;
        HandCount.text = "����: " + DeckManager.Instance.hand.Count;
        ExtraDrawCount.text = "�������: " + GameData.extraDrawNum;
    }
    */

    public void CancelAllSelect() {
        foreach (GameObject card in cards) {
            CardController cardController = card.GetComponent<CardController>();
            if (cardController != null) {
                cardController.isSelected = false;
                cardController.UpdateCard();
            }
        }
    }
}



