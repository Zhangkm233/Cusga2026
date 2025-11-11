using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// 地块数据 挂载在每个Tile物体上
/// </summary>
public class TileController : MonoBehaviour
{
    public Land land;
    public int tileRow;
    public int tileCol;

    private void Start() {
        // 延迟初始化，等待MapManager准备就绪
        StartCoroutine(DelayedInit());
    }
    
    private System.Collections.IEnumerator DelayedInit() {
        // 等待MapManager初始化完成
        while (MapManager.Instance == null || MapManager.Instance.LandMap == null || MapManager.Instance.LandMap.Count == 0) {
            yield return null;
        }

        // 检查坐标是否在有效范围内
        if (tileRow < 0 || tileRow >= MapManager.Instance.RowCount || tileCol < 0 || tileCol >= MapManager.Instance.ColCount) {
            Debug.LogError($"TileController {gameObject.name} 坐标超出范围: ({tileRow}, {tileCol})，地图大小: 3x4");
            yield break;
        }
        
        UpdateLand();
        MapManager.Instance.OnLandChanged.AddListener((row,col) => {
            if (row == tileRow && col == tileCol) {
                UpdateLand();
            }
        });
    }

    public Land GetLand() {
        // 检查坐标是否在有效范围内
        if (tileRow < 0 || tileRow >= MapManager.Instance.RowCount || tileCol < 0 || tileCol >= MapManager.Instance.ColCount) {
            Debug.LogError($"TileController {gameObject.name} 坐标超出范围: ({tileRow}, {tileCol})，地图大小: 3x4");
            return null;
        }
        return MapManager.Instance.GetLandAt(tileRow,tileCol);
    }

    void UpdateLand() {
        land = GetLand();
    }

    
    // 接受卡片放置
    public void OnCardPlaced(Card card) {
        if (land != null && card != null) {
            Debug.Log($"卡片 {card.CardName} 放置到格子 ({tileRow}, {tileCol})");
            
            // 应用卡片效果
            if (card is MaterialCard materialCard) {
                land.MaterialEffect(materialCard);
            } else {
                card.ApplyEffect(land);
            }
        }
    }

    public bool IsThisPlaceExist() {
        return MapManager.Instance.IsPlaceLegal(tileRow,tileCol);
    }
}
