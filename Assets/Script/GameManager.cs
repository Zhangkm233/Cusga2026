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

    /*
     *  回合开始阶段-触发回合开始时扳机
        1、
        抽卡阶段-抽取4张资源卡，3张技能卡
        额外抽卡阶段-按照额外抽卡效果，额外抽取对应卡牌
        2、
        用卡阶段-玩家在该阶段中可以使用自己手中的资源卡和技能卡，点击回合结束按钮后才会进入下一阶段
        3、
        天灾阶段-回合结束时，销毁手中剩余的资源卡和技能卡，如果剩余了天灾卡则触发天灾
        动物阶段-所有活着的动物移动到自身所在格与相邻四格内所有空旷的格子中，最接近自己偏好地势数值的地块
        充能阶段-仅当boss战未触发时执行，使场上所有地块获得1充能，如果有充能已满的地块，触发其效果
        额外收益阶段-如果有每回合触发的额外收益，在此时进行判断并执行
        
        Boss攻击阶段-仅当boss战触发时执行，boss进行攻击
        回合结束阶段-执行回合结束扳机，最后回合数+1，如果回合已满则出现boss

    */

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
        //抽卡阶段-抽取4张资源卡，3张技能卡
        DrawPhase();
        //额外抽卡阶段-按照额外抽卡效果，额外抽取对应卡牌
        ExtraDrawPhase();
        //UIManager.Instance.UpdateCards();
    }

    public void DrawPhase() {
        // 抽卡阶段逻辑
        Debug.Log("抽卡阶段");
        // 这里可以添加更多的逻辑，比如更新UI，处理状态等
        DeckManager.Instance.ShuffleDeck();
        for (int i = 0;i < 4;i++) {
            DeckManager.Instance.DrawCertainCardByType(CardType.MATERIAL);
        }
        for (int i = 0;i < 3;i++) {
            DeckManager.Instance.DrawCertainCardByType(CardType.SKILL);
        }
    }

    public void ExtraDrawPhase() {
        //先抽特定ID的
        for (int i = 0;i < DeckManager.Instance.extraCertainCardId.Count;i++) {
            DeckManager.Instance.DrawCertainCardById(DeckManager.Instance.extraCertainCardId[i]);
        }
        DeckManager.Instance.extraCertainCardId.Clear();
        //再抽特定种类的
        for (int i = 0;i < DeckManager.Instance.extraCertainCardType.Count;i++) {
            DeckManager.Instance.DrawCertainCardByType(DeckManager.Instance.extraCertainCardType[i]);
        }
        DeckManager.Instance.extraCertainCardType.Clear();
        //再抽额外的
        for (int i = 0;i < DeckManager.Instance.ExtraDrawNum;i++) {
            DeckManager.Instance.DrawCard();
        }
    }
}
