using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TileController : MonoBehaviour
{
    //游戏层绑定在Tile上的脚本 用于同步Land 更新数值显示
    public Land land;
    public int tileRow;
    public int tileCol;

    public Land GetLand() {
        MapManager.Instance.GetLandAt(tileRow,tileCol);
        return land;
    }

    void UpdateLand() {
        land = GetLand();
    }
}
