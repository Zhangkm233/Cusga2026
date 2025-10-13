// using UnityEngine;

// /// <summary>
// /// 章节管理器
// /// 负责管理章节进度和触发局外成长选择
// /// </summary>
// public class ChapterManager : MonoBehaviour
// {
//     public static ChapterManager Instance { get; private set; }
    
//     [Header("章节配置")]
//     [SerializeField] private int currentChapter = 1;
//     [SerializeField] private bool isChapterCompleted = false;
    
//     [Header("UI引用")]
//     [SerializeField] private ProgressionUI progressionUI;
    
//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
    
//     /// <summary>
//     /// 完成当前章节
//     /// </summary>
//     public void CompleteChapter()
//     {
//         if (isChapterCompleted) return;
        
//         isChapterCompleted = true;
//         Debug.Log($"第{currentChapter}章完成！");
        
//         // 解锁该章节的升级效果
//         ProgressionManager.Instance.UnlockChapterUpgrades(currentChapter);
        
//         // 显示升级选择界面
//         ShowProgressionSelection();
//     }
    
//     /// <summary>
//     /// 显示升级选择界面
//     /// </summary>
//     private void ShowProgressionSelection()
//     {
//         if (progressionUI != null)
//         {
//             progressionUI.ShowProgressionSelection(currentChapter);
//         }
//         else
//         {
//             Debug.LogWarning("ProgressionUI未设置，无法显示升级选择界面");
//         }
//     }
    
//     /// <summary>
//     /// 进入下一章节
//     /// </summary>
//     public void EnterNextChapter()
//     {
//         currentChapter++;
//         isChapterCompleted = false;
//         Debug.Log($"进入第{currentChapter}章");
        
//         // 这里可以触发新章节的初始化逻辑
//         InitializeNewChapter();
//     }
    
//     /// <summary>
//     /// 初始化新章节
//     /// </summary>
//     private void InitializeNewChapter()
//     {
//         // 重置游戏状态
//         GameData.turnCount = 0;
//         GameData.IsBossSpawned = false;
        
//         // 重新初始化地图、牌库等
//         if (MapManager.Instance != null)
//         {
//             MapManager.Instance.InitializeMap();
//         }
        
//         if (DeckManager.Instance != null)
//         {
//             DeckManager.Instance.InitializingData();
//             DeckManager.Instance.TestAddCard();
//         }
        
//         Debug.Log($"第{currentChapter}章初始化完成");
//     }
    
//     /// <summary>
//     /// 获取当前章节
//     /// </summary>
//     /// <returns>当前章节号</returns>
//     public int GetCurrentChapter()
//     {
//         return currentChapter;
//     }
    
//     /// <summary>
//     /// 检查章节是否已完成
//     /// </summary>
//     /// <returns>是否已完成</returns>
//     public bool IsChapterCompleted()
//     {
//         return isChapterCompleted;
//     }
// }
