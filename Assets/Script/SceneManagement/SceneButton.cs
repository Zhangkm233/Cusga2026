using UnityEngine;

/// <summary>
/// 场景按钮控制器 - 用于在目标场景中触发场景切换
/// </summary>
public class SceneButton : MonoBehaviour
{
    [Header("目标场景")]
    [SerializeField] private string targetScene;
    
    /// <summary>
    /// 跳转到指定场景（通过Inspector设置）
    /// </summary>
    public void LoadTargetScene()
    {
        if (SceneManager.Instance != null)
        {
            Debug.Log($"🎯 通过SceneButton加载场景: {targetScene}");
            SceneManager.Instance.LoadScene(targetScene);
        }
        else
        {
            Debug.LogError("❌ 找不到SceneManager实例！");
        }
    }
    
    /// <summary>
    /// 跳转到主菜单
    /// </summary>
    public void LoadMainMenu()
    {
        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.LoadMainMenu();
        }
        else
        {
            Debug.LogError("❌ 找不到SceneManager实例！");
        }
    }
    
    /// <summary>
    /// 跳转到进度场景
    /// </summary>
    public void LoadProgression()
    {
        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.LoadProgression();
        }
        else
        {
            Debug.LogError("❌ 找不到SceneManager实例！");
        }
    }
    
    /// <summary>
    /// 跳转到游戏场景
    /// </summary>
    public void LoadGamePlay()
    {
        if (SceneManager.Instance != null)
        {
            SceneManager.Instance.LoadGamePlay();
        }
        else
        {
            Debug.LogError("❌ 找不到SceneManager实例！");
        }
    }
}
