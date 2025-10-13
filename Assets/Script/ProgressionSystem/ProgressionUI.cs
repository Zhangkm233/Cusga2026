using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 局外成长选择界面UI控制器
/// 负责显示升级选择界面，处理玩家的选择
/// </summary>
public class ProgressionUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject progressionPanel;
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI chapterTitle;
    [SerializeField] private TextMeshProUGUI instructionText;
    
    [Header("选择配置")]
    [SerializeField] private int maxSelections = 3; // 最多选择3个升级
    [SerializeField] private int currentChapter = 1;
    
    private List<ProgressionUpgrade> availableUpgrades;
    private List<ProgressionUpgrade> selectedUpgrades = new List<ProgressionUpgrade>();
    private List<GameObject> upgradeButtons = new List<GameObject>();
    
    private void Start()
    {
        // 初始化UI
        progressionPanel.SetActive(false);
        confirmButton.onClick.AddListener(ConfirmSelections);
        
        // 设置章节标题
        chapterTitle.text = $"第{currentChapter}章 - 局外成长";
        instructionText.text = $"请选择{maxSelections}个升级效果：";
    }
    
    /// <summary>
    /// 显示指定章节的升级选择界面
    /// </summary>
    /// <param name="chapter">章节号</param>
    public void ShowProgressionSelection(int chapter)
    {
        currentChapter = chapter;
        chapterTitle.text = $"第{chapter}章 - 局外成长";
        
        // 获取可选择的升级
        availableUpgrades = ProgressionManager.Instance.GetAvailableUpgradesForChapter(chapter);
        
        if (availableUpgrades.Count == 0)
        {
            Debug.LogWarning($"第{chapter}章没有可选择的升级效果");
            return;
        }
        
        // 显示界面
        progressionPanel.SetActive(true);
        
        // 创建升级选择按钮
        CreateUpgradeButtons();
        
        // 重置选择状态
        selectedUpgrades.Clear();
        UpdateConfirmButton();
    }
    
    /// <summary>
    /// 创建升级选择按钮
    /// </summary>
    private void CreateUpgradeButtons()
    {
        // 清除现有按钮
        foreach (var button in upgradeButtons)
        {
            Destroy(button);
        }
        upgradeButtons.Clear();
        
        // 创建新按钮
        foreach (var upgrade in availableUpgrades)
        {
            GameObject buttonObj = Instantiate(upgradeButtonPrefab, upgradeContainer);
            upgradeButtons.Add(buttonObj);
            
            // 设置按钮内容
            var button = buttonObj.GetComponent<Button>();
            var nameText = buttonObj.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            var descText = buttonObj.transform.Find("DescText").GetComponent<TextMeshProUGUI>();
            var selectImage = buttonObj.transform.Find("SelectImage").GetComponent<Image>();
            
            nameText.text = upgrade.upgradeName;
            descText.text = upgrade.upgradeDescription;
            selectImage.gameObject.SetActive(false);
            
            // 设置按钮点击事件
            button.onClick.AddListener(() => ToggleUpgradeSelection(upgrade, selectImage));
        }
    }
    
    /// <summary>
    /// 切换升级选择状态
    /// </summary>
    /// <param name="upgrade">升级效果</param>
    /// <param name="selectImage">选择图标</param>
    private void ToggleUpgradeSelection(ProgressionUpgrade upgrade, Image selectImage)
    {
        if (selectedUpgrades.Contains(upgrade))
        {
            // 取消选择
            selectedUpgrades.Remove(upgrade);
            selectImage.gameObject.SetActive(false);
        }
        else
        {
            // 检查是否已达到最大选择数量
            if (selectedUpgrades.Count >= maxSelections)
            {
                Debug.Log($"最多只能选择{maxSelections}个升级效果");
                return;
            }
            
            // 添加选择
            selectedUpgrades.Add(upgrade);
            selectImage.gameObject.SetActive(true);
        }
        
        UpdateConfirmButton();
    }
    
    /// <summary>
    /// 更新确认按钮状态
    /// </summary>
    private void UpdateConfirmButton()
    {
        bool canConfirm = selectedUpgrades.Count == maxSelections;
        confirmButton.interactable = canConfirm;
        
        if (canConfirm)
        {
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = "确认选择";
        }
        else
        {
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = 
                $"还需选择 {maxSelections - selectedUpgrades.Count} 个升级";
        }
    }
    
    /// <summary>
    /// 确认选择并激活升级效果
    /// </summary>
    private void ConfirmSelections()
    {
        if (selectedUpgrades.Count != maxSelections)
        {
            Debug.LogWarning("选择数量不正确");
            return;
        }
        
        // 激活选择的升级效果
        foreach (var upgrade in selectedUpgrades)
        {
            ProgressionManager.Instance.ActivateUpgrade(upgrade.upgradeType);
        }
        
        // 保存升级状态
        ProgressionManager.Instance.SaveProgressionData();
        
        // 隐藏界面
        progressionPanel.SetActive(false);
        
        // 显示选择结果
        string resultText = "已激活的升级效果：\n";
        foreach (var upgrade in selectedUpgrades)
        {
            resultText += $"• {upgrade.upgradeName}\n";
        }
        Debug.Log(resultText);
        
        // 触发游戏继续或其他逻辑
        OnProgressionSelectionComplete();
    }
    
    /// <summary>
    /// 升级选择完成回调
    /// </summary>
    private void OnProgressionSelectionComplete()
    {
        // 这里触发游戏继续、场景切换等逻辑
        Debug.Log("升级选择完成，游戏继续");
        
        // 示例：切换到游戏场景
        // SceneManager.Instance.LoadGamePlay();
    }
    
    /// <summary>
    /// 隐藏升级选择界面
    /// </summary>
    public void HideProgressionPanel()
    {
        progressionPanel.SetActive(false);
    }
}
