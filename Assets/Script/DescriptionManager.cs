using UnityEngine;

public class DescriptionManager : MonoBehaviour
{
    //管理卡片/地块描述框的显示与隐藏 用单例模式保证同时只有一个描述框
    public static DescriptionManager Instance { get; private set; }
    public GameObject descriptionPanel; //描述框UI
    public TMPro.TextMeshProUGUI descriptionText; //描述文本组件

    public bool isDragging = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void ShowDescription(string description) {
        descriptionText.text = description;
        descriptionPanel.SetActive(true);
    }
    public void HideDescription() {
        descriptionPanel.SetActive(false);
    }

}
