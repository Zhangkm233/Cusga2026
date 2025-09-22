using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//已弃用
[System.Obsolete("需要重写")]
public class UIManager : MonoBehaviour
{
    // 这里也需要重写 改成3d
    // 控制游戏内的UI显示
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
    private void Start() {
        foreach (var card in cards) {
            card.GetComponent<CardController>().UpdateSortingOrder();
        }
    }

    public void UpdateCards() {
        //GameObject[] cards = GameObject.FindGameObjectsWithTag("CardGameobject");
        for (int i = 0;i < DeckManager.Instance.hand.Count;i++) {
            CardController cardController = cards[i].GetComponent<CardController>();
            cardController.card = DeckManager.Instance.hand[i];
            cardController.UpdateCard();
        }
    }

    /*
    public void UpdateTiles() {
        // 获取所有TileGameobject对象并更新它们的状态
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("TileGameobject");
        foreach (GameObject tile in tiles) {
            tile.GetComponent<TileController>().UpdateTile();
        }
    }

    public void UpdateUI() {
        DeckCount.text = "牌库: " + DeckManager.Instance.deck.Count;
        HandCount.text = "手牌: " + DeckManager.Instance.hand.Count;
        ExtraDrawCount.text = "额外抽牌: " + GameData.extraDrawNum;
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
