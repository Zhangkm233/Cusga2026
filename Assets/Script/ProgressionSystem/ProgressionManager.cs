using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 局外成长系统管理器
/// 负责管理所有章节的升级效果，提供统一的接口供游戏系统调用
/// </summary>
public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance { get; private set; }
    
    [Header("升级效果配置")]
    [SerializeField] private List<ProgressionUpgrade> allUpgrades = new List<ProgressionUpgrade>();
    
    [Header("当前激活的升级")]
    [SerializeField] private List<ProgressionUpgradeType> activeUpgrades = new List<ProgressionUpgradeType>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 初始化所有升级效果
    /// </summary>
    private void InitializeUpgrades()
    {
        allUpgrades.Clear();
        
        // 第一章升级效果
        allUpgrades.Add(new ProgressionUpgrade(
            ProgressionUpgradeType.MOUNTAIN_EFFICIENCY,
            "山脉效率提升",
            "山脉产出石头所需的充能降低为3",
            1, 1
        ));
        
        allUpgrades.Add(new ProgressionUpgrade(
            ProgressionUpgradeType.REINFORCE_EXTRA_CARD,
            "加固技能强化",
            "获得加固技能的时候，额外将1张置入牌库",
            1, 2
        ));
        
        allUpgrades.Add(new ProgressionUpgrade(
            ProgressionUpgradeType.FOREST_BONUS_CHANCE,
            "森林丰收",
            "森林产出木头时，有5%概率额外产出2张",
            1, 3
        ));
        
        allUpgrades.Add(new ProgressionUpgrade(
            ProgressionUpgradeType.ROLLROCK_DAMAGE_BOOST,
            "滚石强化",
            "滚石伤害最大值+1",
            1, 4
        ));
        
        // 这里可以继续添加第二章、第三章的升级效果
        // 第二章示例 (预留)
        // allUpgrades.Add(new ProgressionUpgrade(
        //     ProgressionUpgradeType.PLAIN_ENERGY_BOOST,
        //     "平原能量增强",
        //     "平原地块额外产出1能量",
        //     2, 1
        // ));
    }
    
    /// <summary>
    /// 解锁指定章节的所有升级效果
    /// </summary>
    /// <param name="chapter">章节号</param>
    public void UnlockChapterUpgrades(int chapter)
    {
        var chapterUpgrades = allUpgrades.Where(u => u.chapterUnlocked == chapter).ToList();
        foreach (var upgrade in chapterUpgrades)
        {
            upgrade.isUnlocked = true;
            Debug.Log($"解锁升级: {upgrade.upgradeName}");
        }
    }
    
    /// <summary>
    /// 获取指定章节可选择的升级效果
    /// </summary>
    /// <param name="chapter">章节号</param>
    /// <returns>可选择的升级效果列表</returns>
    public List<ProgressionUpgrade> GetAvailableUpgradesForChapter(int chapter)
    {
        return allUpgrades
            .Where(u => u.chapterUnlocked == chapter && u.isUnlocked && !u.isActive)
            .OrderBy(u => u.priority)
            .ToList();
    }
    
    /// <summary>
    /// 激活指定的升级效果
    /// </summary>
    /// <param name="upgradeType">升级类型</param>
    public void ActivateUpgrade(ProgressionUpgradeType upgradeType)
    {
        var upgrade = allUpgrades.FirstOrDefault(u => u.upgradeType == upgradeType);
        if (upgrade != null && upgrade.isUnlocked && !upgrade.isActive)
        {
            upgrade.isActive = true;
            activeUpgrades.Add(upgradeType);
            Debug.Log($"激活升级: {upgrade.upgradeName}");
        }
    }
    
    /// <summary>
    /// 检查指定升级是否已激活
    /// </summary>
    /// <param name="upgradeType">升级类型</param>
    /// <returns>是否已激活</returns>
    public bool IsUpgradeActive(ProgressionUpgradeType upgradeType)
    {
        return activeUpgrades.Contains(upgradeType);
    }
    
    /// <summary>
    /// 获取山脉地块的充能需求
    /// 如果激活了山脉效率升级，则返回3，否则返回4
    /// </summary>
    /// <returns>充能需求</returns>
    public int GetMountainRequiredEnergy()
    {
        return IsUpgradeActive(ProgressionUpgradeType.MOUNTAIN_EFFICIENCY) ? 3 : 4;
    }
    
    /// <summary>
    /// 获取滚石武器的最大伤害
    /// 如果激活了滚石强化升级，则返回3，否则返回2
    /// </summary>
    /// <returns>最大伤害</returns>
    public int GetRollrockMaxDamage()
    {
        return IsUpgradeActive(ProgressionUpgradeType.ROLLROCK_DAMAGE_BOOST) ? 3 : 2;
    }
    
    /// <summary>
    /// 检查森林是否应该触发额外产出
    /// </summary>
    /// <returns>是否触发额外产出</returns>
    public bool ShouldTriggerForestBonus()
    {
        return IsUpgradeActive(ProgressionUpgradeType.FOREST_BONUS_CHANCE) && 
               GameData.IsRandomEventTriggered(5);
    }
    
    /// <summary>
    /// 应用加固技能效果
    /// 如果激活了加固强化升级，会额外添加一张加固卡到牌库
    /// </summary>
    /// <param name="targetLand">目标地块</param>
    public void ApplyReinforceEffect(Land targetLand)
    {
        // 基础效果：添加土壤
        targetLand.AddSoild(1);
        
        // 如果激活了加固强化升级，额外添加一张加固卡
        if (IsUpgradeActive(ProgressionUpgradeType.REINFORCE_EXTRA_CARD))
        {
            DeckManager.Instance.AddCardToDeck(new SkillCard(SkillType.REINFORCE), 1);
            Debug.Log("加固技能强化：额外添加一张加固卡到牌库");
        }
    }
    
    /// <summary>
    /// 获取所有已激活的升级效果
    /// </summary>
    /// <returns>已激活的升级效果列表</returns>
    public List<ProgressionUpgrade> GetActiveUpgrades()
    {
        return allUpgrades.Where(u => u.isActive).ToList();
    }
    
    /// <summary>
    /// 重置所有升级状态（用于新游戏）
    /// </summary>
    public void ResetAllUpgrades()
    {
        foreach (var upgrade in allUpgrades)
        {
            upgrade.isUnlocked = false;
            upgrade.isActive = false;
        }
        activeUpgrades.Clear();
        Debug.Log("重置所有升级状态");
    }
    
    /// <summary>
    /// 保存升级状态到存档
    /// </summary>
    public void SaveProgressionData()
    {
        // TODO: 实现存档功能
        // 可以使用PlayerPrefs或JSON文件保存activeUpgrades列表
        Debug.Log("保存升级状态到存档");
    }
    
    /// <summary>
    /// 从存档加载升级状态
    /// </summary>
    public void LoadProgressionData()
    {
        // TODO: 实现读档功能
        // 从存档中读取activeUpgrades列表并恢复状态
        Debug.Log("从存档加载升级状态");
    }
}
