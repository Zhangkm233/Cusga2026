using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

//已弃用
[System.Obsolete("需要重写")]
public class TileController : MonoBehaviour
{
    // 这里需要重写 改成3d 
    // 控制游戏内的地块显示
    public TMP_Text tileName;
    public TMP_Text tileEnergy;
    public TMP_Text tileAtk;
    public TMP_Text tileDef;
    public TMP_Text tileStorageNum;
    public TMP_Text tileStorageCard;
    public Land land;
    public GameObject tileCanvas;
    void Awake(){
        //随机添加山丘 平原 森林三种地形之一
        int randomTerrain = Random.Range(0,3);
        switch (randomTerrain) {
            case 0:
                land = this.AddComponent<HillLand>();
                break;
            case 1:
                land = this.AddComponent<PlainLand>();
                break;
            case 2:
                land = this.AddComponent<ForestLand>();
                break;
        }


    }

    private void Start() {
        if (land == null) {
            land = this.AddComponent<Land>();
        }
        if (tileCanvas == null) {
            tileCanvas = transform.GetComponentInChildren<Canvas>().gameObject;
        }
        if (tileName == null) {
            tileName = tileCanvas.transform.Find("TileName").GetComponent<TMP_Text>();
        }
        if (tileEnergy == null) {
            tileEnergy = tileCanvas.transform.Find("TileEnergy").GetComponent<TMP_Text>();
        }
        if (tileAtk == null) {
            tileAtk = tileCanvas.transform.Find("TileAtk").GetComponent<TMP_Text>();
        }
        if (tileDef == null) {
            tileDef = tileCanvas.transform.Find("TileDef").GetComponent<TMP_Text>();
        }
        if (tileStorageCard == null) {
            tileStorageCard = tileCanvas.transform.Find("TileStorageCard").GetComponent<TMP_Text>();
        }
        if (tileStorageNum == null) {
            tileStorageNum = tileCanvas.transform.Find("TileStorageNum").GetComponent<TMP_Text>();
        }
        UpdateTile();
    }

    public void UpdateTile() {
        land = this.GetComponent<Land>();
        tileName.text = GameData.HanizeLandType(land.LandType);
        tileEnergy.text = "能量: " + land.EnergyCounter.ToString();
        //tileAtk.text = "攻击: " + land.atk.ToString();
        //tileDef.text = "护盾: " + land.def.ToString();
        if (land.storageCardType != MaterialType.NULL || land.storageCardNum == 0) {
            tileStorageCard.text = GameData.HanizeMaterial(land.storageCardType);
            tileStorageNum.text = land.storageCardNum.ToString();
        } else {
            tileStorageCard.text = " ";
            tileStorageNum.text = " ";
        }
    }
}
