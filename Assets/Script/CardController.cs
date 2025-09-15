using Mono.Cecil;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CardController : MonoBehaviour
{
    //控制游戏内的卡牌显示
    //这里需要重写
    public TMP_Text cardName;
    public TMP_Text cardDescription;
    public TMP_Text cardType;
    public Card card;
    public GameObject cardCanvas;
    public int indexOfCards;
    public bool isSelected = false;

    public CardController(Card card) {
        this.card = card;
    }

    private void Start() {
        cardCanvas = transform.GetComponentInChildren<Canvas>().gameObject;
        if (cardName == null) {
            cardName = cardCanvas.transform.Find("CardName").GetComponent<TMP_Text>();
        }
        if (cardDescription == null) {
            cardDescription = cardCanvas.transform.Find("CardDes").GetComponent<TMP_Text>();
        }
        if (cardType == null) {
            cardType = cardCanvas.transform.Find("CardType").GetComponent<TMP_Text>();
        }
        UpdateCard();
    }

    public void UpdateCard() {

        if (DeckManager.Instance.hand.Count <= indexOfCards) {
            card = null;
        } else {
            card = DeckManager.Instance.hand[indexOfCards];
        }

        if (card != null) {
            this.gameObject.SetActive(true);
            cardName.text = card.Name;
            cardDescription.text = card.Description;
            cardType.text = GameData.HanizeCardType(card.CardType);
        } else {
            this.gameObject.SetActive(false);
            cardName.text = "无";
            cardDescription.text = "无描述";
            cardType.text = " ";
        }
        this.GetComponent<SpriteRenderer>().sortingOrder = indexOfCards;
        cardCanvas.GetComponent<Canvas>().sortingOrder = indexOfCards;
        if (isSelected) {
            GetComponent<SpriteRenderer>().color = new Color(1f,1f,0.5f,1f); // 选中时变为黄色
            UpdateSortingOrder(50);
        } else {
            GetComponent<SpriteRenderer>().color = Color.white; // 未选中时恢复为白色
            UpdateSortingOrder();
        }
    }

    public void UpdateSortingOrder() {
        // 更新卡片的渲染顺序
        this.GetComponent<SpriteRenderer>().sortingOrder = indexOfCards;
        cardCanvas.GetComponent<Canvas>().sortingOrder = indexOfCards;
        this.GetComponent<BoxCollider>().layerOverridePriority = indexOfCards;
    }

    public void UpdateSortingOrder(int order) {
        // 更新卡片的渲染顺序
        this.GetComponent<SpriteRenderer>().sortingOrder = order;
        cardCanvas.GetComponent<Canvas>().sortingOrder = order;
        this.GetComponent<BoxCollider>().layerOverridePriority = order;
    }
}
