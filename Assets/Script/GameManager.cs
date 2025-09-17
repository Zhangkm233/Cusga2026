using UnityEngine;

//已弃用
[System.Obsolete("需要重写")]
public class GameManager : MonoBehaviour
{
    //这里需要重写 要记得和逻辑层解耦合
    //用于管理游戏内的整体流程
    public GameObject selectCard;
    public static GameManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    private void Start() {
        // 初始化游戏数据
        DeckManager.Instance.InitializingData();
        StartGame();
    }

    private void Update() {
        // 每帧检测鼠标点击
        DetectMouse();
        // 这里可以添加其他游戏逻辑，比如更新UI等
    }

    public void DetectMouse() {
        // 检测鼠标点击逻辑
        /*
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray,out hit)) {
                GameObject clickedObject = hit.collider?.gameObject;
                if (clickedObject.CompareTag("TileGameobject") && selectCard != null) {
                    int temp = DeckManager.Instance.hand.Count;
                    bool Costed = false;
                    if (selectCard.GetComponent<CardController>().card.CardType == CardType.MATERIAL) {
                        clickedObject.GetComponent<TileController>().land.MaterialEffect((MaterialCard)selectCard.GetComponent<CardController>().card);
                        Costed = true;
                    }
                    if (selectCard.GetComponent<CardController>().card.CardType == CardType.SKILL) {
                        ((SkillCard)selectCard.GetComponent<CardController>().card).ApplyEffect(clickedObject.GetComponent<TileController>().land);
                        Costed = true;
                    }
                    if (Costed) {
                        DeckManager.Instance.hand.Remove(selectCard.GetComponent<CardController>().card);
                        selectCard.GetComponent<CardController>().card = null; // 清除选中的卡片
                        //UIManager.Instance.cards[temp - 1].GetComponent<CardController>().UpdateCard(); // 更新UI
                    }
                    UIManager.Instance.CancelAllSelect();
                    selectCard = null;
                }
                if (clickedObject.CompareTag("CardGameobject")) {
                    CardController cardController = clickedObject.GetComponent<CardController>();
                    if (cardController.card != null) {
                        // 处理卡片点击逻辑
                        Debug.Log($"选定了卡片: {cardController.card.name}");
                        selectCard = clickedObject;
                        // 这里可以添加更多的逻辑，比如使用卡片等
                        UIManager.Instance.CancelAllSelect();
                        clickedObject.GetComponent<CardController>().isSelected = true;
                    }
                }
            } else {
                UIManager.Instance.CancelAllSelect();
                selectCard = null; // 如果没有点击到任何对象，取消选择
                Debug.Log("没有点击到任何对象");
            }
        }
        */
    }

    public void StartGame() {
        // 游戏开始逻辑
        Debug.Log("游戏开始");
        DeckManager.Instance.ShuffleDeck();
        DeckManager.Instance.DrawCard();
    }

    public void EndTurn() {
        // 结束回合逻辑
        Debug.Log("结束回合");
        // 这里可以添加更多的逻辑，比如更新UI，处理状态等
        DeckManager.Instance.ClearHand();
        // 每个地块能量+1 然后触发被动效果

        // 这里需要重写
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("TileGameobject");
        foreach (GameObject tile in tiles) {
            tile.GetComponent<Land>().EnergyCounter++;
            tile.GetComponent<Land>().PassiveEffect();
        }
        //额外效果触发
        foreach (GameObject tile in tiles) {
            //tile.GetComponent<Land>().ExtraEffect();
        }
        //UIManager.Instance.UpdateTiles();
        //UIManager.Instance.UpdateCards();
        StartTurn();
    }

    public void StartTurn() {
        // 开始回合逻辑
        Debug.Log("开始回合");
        DeckManager.Instance.ShuffleDeck();
        for (int i = 0;i < 4 + GameData.extraDrawNum;i++) {
            DeckManager.Instance.DrawCard();
        }
        GameData.extraDrawNum = 0;
        //UIManager.Instance.UpdateCards();
    }
}
