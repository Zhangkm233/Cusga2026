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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
}