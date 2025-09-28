using UnityEngine;


public class GameManager : MonoBehaviour
{
    //������Ҫ��д Ҫ�ǵú��߼�������
    //���ڹ�����Ϸ�ڵ���������
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
        // ��ʼ����Ϸ����
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
        // ��Ϸ��ʼ�߼�
        stateMachine.ChangePhase(GamePhase.GameStart);
    }

    /*
     *  �غϿ�ʼ�׶�-�����غϿ�ʼʱ���
        1��
        �鿨�׶�-��ȡ4����Դ����3�ż��ܿ�
        ����鿨�׶�-���ն���鿨Ч���������ȡ��Ӧ����
        2��
        �ÿ��׶�-����ڸý׶��п���ʹ���Լ����е���Դ���ͼ��ܿ�������غϽ�����ť��Ż������һ�׶�
        3��
        ���ֽ׶�-�غϽ���ʱ����������ʣ�����Դ���ͼ��ܿ������ʣ�������ֿ��򴥷�����
        ����׶�-���л��ŵĶ����ƶ����������ڸ��������ĸ������пտ��ĸ����У���ӽ��Լ�ƫ�õ�����ֵ�ĵؿ�
        ���ܽ׶�-����bossսδ����ʱִ�У�ʹ�������еؿ���1���ܣ�����г��������ĵؿ飬������Ч��
        ��������׶�-�����ÿ�غϴ����Ķ������棬�ڴ�ʱ�����жϲ�ִ��
        
        Boss�����׶�-����bossս����ʱִ�У�boss���й���
        �غϽ����׶�-ִ�лغϽ�����������غ���+1������غ����������boss ��������������ֵ�Ч��

    */

    public void EndTurn() {
        // �����غ��߼�
        Debug.Log("�����غ�");
        // ����������Ӹ�����߼����������UI������״̬��
        if(stateMachine.CurrentPhase == GamePhase.PlayerTurn) {
            stateMachine.ChangePhase(GamePhase.TurnEnd);
        }
    }

    public void StartTurn() {
        // ��ʼ�غ��߼�
        Debug.Log("��ʼ�غ�");
        stateMachine.ChangePhase(GamePhase.TurnStart);
    }
}
