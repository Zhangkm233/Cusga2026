using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 场景转换器 - 直接挂载在按钮上使用
/// </summary>
public class SceneTransition : MonoBehaviour
{
    [Header("场景转换设置")]
    [SerializeField] private SceneType sceneType;
    [SerializeField] private string customSceneName;
    
    private Button button;
    
    public enum SceneType
    {
        MainMenu,
        Progression,
        GamePlay,
        Custom
    }
    
    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogWarning("⚠️ SceneTransition脚本需要挂载在Button组件上");
        }
    }
    
    public void LoadScene()
    {
        if (SceneManager.Instance == null)
        {
            Debug.LogError("❌ SceneManager实例不存在！");
            return;
        }
        
        switch (sceneType)
        {
            case SceneType.MainMenu:
                SceneManager.Instance.LoadMainMenu();
                break;
            case SceneType.Progression:
                SceneManager.Instance.LoadProgression();
                break;
            case SceneType.GamePlay:
                SceneManager.Instance.LoadGamePlay();
                break;
            case SceneType.Custom:
                if (!string.IsNullOrEmpty(customSceneName))
                {
                    SceneManager.Instance.LoadScene(customSceneName);
                }
                else
                {
                    Debug.LogError("❌ 自定义场景名称不能为空！");
                }
                break;
        }
    }
}
