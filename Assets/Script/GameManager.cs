using UnityEngine;


public class GameManager : MonoBehaviour
{
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
        DeckManager.Instance.InitializingData(); 
        
        // 初始化地图数据
        if (MapManager.Instance != null) {
            MapManager.Instance.InitiallizeLandMap(); // 使用默认的3x4地图
        }
        
        stateMachine = new GameStateMachine();
        stateMachine.ChangePhase(GamePhase.GamePaused);
        StartGame();
    }

    private void Update() {
        stateMachine.Update();
    }

    public void StartGame() {
        stateMachine.ChangePhase(GamePhase.GameStart);
    }


    public void EndTurn() {
        Debug.Log("结束回合");
        if(stateMachine.CurrentPhase == GamePhase.PlayerTurn) {
            stateMachine.ChangePhase(GamePhase.TurnEnd);
        }
    }

    public void StartTurn() {
        Debug.Log("开始回合");
        stateMachine.ChangePhase(GamePhase.TurnStart);
    }

    public void PauseGame() {
        Debug.Log("游戏暂停");
        stateMachine.ChangePhase(GamePhase.GamePaused);
    }

    public void ContinueGame() {
        Debug.Log("继续游戏");
        stateMachine.ChangePhase(GamePhase.PlayerTurn);
    }
}
