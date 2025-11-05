using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void WeaponDealDamage(Land targetLand,int damage) {
        if (targetLand == null) {
            ApplyDamage(BossManager.Instance.CurrentBoss,damage);
            Debug.Log($"对Boss造成了{damage}点伤害");
        } else {
            if (MapManager.Instance.AnimalMap[targetLand.MapRow][targetLand.MapCol] == null) {
                Debug.LogWarning("该地形上没有动物，无法造成伤害");
                return;
            } else {
                ApplyDamage(MapManager.Instance.AnimalMap[targetLand.MapRow][targetLand.MapCol],damage);
                Debug.Log($"对地形({targetLand.MapRow},{targetLand.MapCol})上的动物造成了{damage}点伤害");
            }
        }
    }

    public void ApplyDamage(IDamageable damageable,int damage) {
         damageable.TakeDamage(damage);
    }
}
