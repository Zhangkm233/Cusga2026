using UnityEngine;


public class GameManager : MonoBehaviour
{
    //这里需要重写 要记得和逻辑层解耦合
    //用于管理游戏内的整体流程
    public GameObject selectCard;

    public GameStateMachine stateMachine;
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
        stateMachine = new GameStateMachine();
        stateMachine.ChangePhase(GamePhase.GamePaused);
        StartGame();
    }

    private void Update() {
        stateMachine.Update();
    }

    public void DetectMouse() {

    }

    public void StartGame() {
        // 游戏开始逻辑
        stateMachine.ChangePhase(GamePhase.GameStart);
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
        回合结束阶段-执行回合结束扳机，最后回合数+1，如果回合已满则出现boss 还有随机添加天灾的效果

    */

    public void EndTurn() {
        // 结束回合逻辑
        Debug.Log("结束回合");
        // 这里可以添加更多的逻辑，比如更新UI，处理状态等
        if(stateMachine.CurrentPhase == GamePhase.PlayerTurn) {
            stateMachine.ChangePhase(GamePhase.TurnEnd);
        }
    }

    public void StartTurn() {
        // 开始回合逻辑
        Debug.Log("开始回合");
        //抽卡阶段-抽取4张资源卡，3张技能卡
        stateMachine.ChangePhase(GamePhase.TurnStart);
    }
}
