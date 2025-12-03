using UnityEngine;
using System;

[Serializable]
public class Boss : IDamageable
{
    [SerializeField]
    private int health;
    [SerializeField]
    private string bossName;
    public event EventHandler<int> OnBossHealthChanged;

    public int Health {
        get { return health; }
        set { health = Mathf.Max(0,value); }
    }
    public string BossName {
        get { return bossName; }
        set { bossName = value; }
    }

    public Boss() {
        health = 0;
        bossName = " 无 ";
    }

    public Boss(int health,string bossName) {
        Health = health;
        BossName = bossName;
    }

    public void TakeDamage(int damage) {
        if(damage <= 0) {
            Debug.LogWarning("伤害值必须大于0");
            return;
        }
        Health -= damage;
        Debug.Log($"Boss受到了{damage}点伤害，当前生命值为{Health}");
        OnBossHealthChanged?.Invoke(this,damage);
        if (Health <= 0) {
            Die();
        }
    }

    public void Die() {
        Debug.Log("Boss死亡");
        //加入boss死亡的逻辑
        GameData.IsBossSpawned = false;
        GameData.bossSpawnTurn = GameData.turnCount + 3;
        OnBossHealthChanged?.Invoke(this,0);
    }
}
