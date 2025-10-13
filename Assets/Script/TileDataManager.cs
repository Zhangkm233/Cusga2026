using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 地块数据显示 先用这个脚本 后面有别的方案再改
/// </summary>
public class TileDataManager : MonoBehaviour
{
    public TMP_Text dataText;
    private TileController tileController;

    void Start() {
        if (dataText == null) {
            dataText = GetComponentInChildren<TMP_Text>();
        }
        if (tileController == null) {
            tileController = GetComponent<TileController>();
        }
    }

    void Update() {
        UpdateDataDisplay();
    }

    public void UpdateDataDisplay() {
        //更新地块信息显示
        if (dataText == null) {
            Debug.LogWarning("Data Text component is not assigned.");
            return;
        }
        if (tileController == null) {
            Debug.LogWarning("TileController component is not assigned.");
            return;
        }
        string displayText = $"({tileController.tileRow}, {tileController.tileCol})";
        if (tileController.land == null) {
            dataText.text = displayText + "\n 无地块";
            return;
        } else {
            displayText += $"{GameData.HanizeLandType(tileController.land.LandType)}\n";
        }
        if (tileController.land.storageCardType != MaterialType.NULL) {
            displayText += $"{GameData.HanizeMaterial(tileController.land.storageCardType)}:{tileController.land.storageCardNum}";
        }
        dataText.text = displayText;
    }
}