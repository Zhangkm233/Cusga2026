using TMPro;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        UpdateBossData();
    }

    [SerializeField]
    public Boss CurrentBoss;

    public TMP_Text NameDisplay;
    public TMP_Text HpDisplay;


    public void HandleBossHealthChanged(object sender,int damage) {
        Debug.Log("接收到事件：Boss受到了" + damage + "点伤害");
        UpdateBossData();
        //可以根据伤害大小来播放不同动画？
        //如果用动画机的话，需要注意受伤和死亡动画的衔接问题（判断一下是否死亡再播动画）
    }

    public void UpdateBossData() {
        Debug.Log("更新Boss数据");
        if (CurrentBoss == null) {
            NameDisplay.text = "无Boss";
            HpDisplay.text = "HP: N/A";
            return;
        }
        if (CurrentBoss != null && CurrentBoss.Health <= 0) {
            CurrentBoss.OnBossHealthChanged -= HandleBossHealthChanged;
            CurrentBoss = null;
            UpdateBossData();
            return;
        }
        NameDisplay.text = CurrentBoss.BossName;
        HpDisplay.text = $"HP: {CurrentBoss.Health}";
    }

    public void SpawnBoss() {
        CurrentBoss = new Boss(5,"希尔瓦娜斯");
        CurrentBoss.OnBossHealthChanged += HandleBossHealthChanged;
        UpdateBossData();
    }

    private void OnEnable() {
        if (CurrentBoss != null) {
            CurrentBoss.OnBossHealthChanged += HandleBossHealthChanged;
        }
    }

    private void OnDisable() {
        if (CurrentBoss != null) {
            CurrentBoss.OnBossHealthChanged -= HandleBossHealthChanged;
        }
    }
}
