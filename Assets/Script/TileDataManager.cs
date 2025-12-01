using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 地块数据显示脚本 挂载在Tile游戏物体上
/// 读TileController的数据并显示在UI上
/// </summary>
public class TileDataManager : MonoBehaviour
{
    public TMP_Text dataText;
    public TMP_Text energyText;
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
        string displayEnergy = "";
        if (tileController.land == null) {
            dataText.text = displayText + "\n 无";
            return;
        } else {
            displayText += $"{GameData.HanizeLandType(tileController.land.LandType)}\n";
        }
        if (tileController.land.storageCardType != MaterialType.NULL) {
            displayText += $"{GameData.HanizeMaterial(tileController.land.storageCardType)}:{tileController.land.storageCardNum}\n";
        }
        if (MapManager.Instance.AnimalMap != null) {
            Animal animal = MapManager.Instance.GetAnimalAt(tileController.tileRow,tileController.tileCol);
            if (animal != null) {
                displayText += $"{animal.AnimalName} HP:{animal.Health}\n";
            }
        }
        if (tileController.land.EnergyCounter != 0) {
            displayEnergy += $"能量:{tileController.land.EnergyCounter}\n";
        }
        if (tileController.land.Soild != 0) {
            displayEnergy += $"加固:{tileController.land.Soild}\n";
        }
        if (tileController.land.Hunterarea != 0) {
            displayEnergy += $"猎圈:{tileController.land.Hunterarea}\n";
        }
        dataText.text = displayText;
        energyText.text = displayEnergy;
    }
}