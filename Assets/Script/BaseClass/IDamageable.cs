using UnityEngine;

/// <summary>
/// 受伤接口
/// 用于卡牌使用的时候判断目标是否可以受伤
/// </summary>
public interface IDamageable 
{ 
    void TakeDamage(int damage);
}
