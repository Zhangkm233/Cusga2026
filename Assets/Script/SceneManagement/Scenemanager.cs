using UnityEditor.SearchService;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }
    
    [Header("场景名称配置")]
    public string mainMenuScene = "MainMenu";
    public string progressionScene = "Progression";
    public string gamePlayScene = "GamePlay";
    
    
    private void Awake()
    {
        
    }
    
    
    // 通用场景切换
    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    
    // 具体场景
    public void LoadMainMenu() => LoadScene(mainMenuScene);
    public void LoadProgression() => LoadScene(progressionScene);
    public void LoadGamePlay() => LoadScene(gamePlayScene);

    // 退出游戏
    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("游戏已退出");
    #else
        Application.Quit();
        Debug.Log("游戏已退出");
    #endif
    }

}