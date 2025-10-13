using System;
using UnityEngine;

/// <summary>
/// 局外成长升级效果枚举
/// 每个升级效果都有唯一的ID，便于存档和扩展
/// </summary>
public enum ProgressionUpgradeType
{
    // 第一章升级效果
    MOUNTAIN_EFFICIENCY = 1001,      // 山脉产出石头所需的充能降低为3
    REINFORCE_EXTRA_CARD = 1002,     // 获得加固技能的时候，额外将1张置入牌库
    FOREST_BONUS_CHANCE = 1003,      // 森林产出木头时，有5%概率额外产出2张
    ROLLROCK_DAMAGE_BOOST = 1004,   // 滚石伤害最大值+1
    
    // 第二章升级效果 (预留)
    // PLAIN_ENERGY_BOOST = 2001,     // 平原额外能量产出
    // HARVEST_EFFICIENCY = 2002,     // 收割技能效果增强
    
    // 第三章升级效果 (预留)
    // DISASTER_RESISTANCE = 3001,    // 灾难抵抗概率提升
    // ANIMAL_SPAWN_BONUS = 3002,     // 动物生成概率提升
}

/// <summary>
/// 局外成长升级效果数据类
/// 存储单个升级效果的所有信息
/// </summary>
[System.Serializable]
public class ProgressionUpgrade
{
    public ProgressionUpgradeType upgradeType;
    public string upgradeName;
    public string upgradeDescription;
    public int chapterUnlocked;  // 解锁章节
    public bool isUnlocked;      // 是否已解锁
    public bool isActive;        // 是否已激活
    public int priority;        // 优先级，用于排序显示
    
    public ProgressionUpgrade(ProgressionUpgradeType type, string name, string description, int chapter, int priority = 0)
    {
        this.upgradeType = type;
        this.upgradeName = name;
        this.upgradeDescription = description;
        this.chapterUnlocked = chapter;
        this.isUnlocked = false;
        this.isActive = false;
        this.priority = priority;
    }
}
