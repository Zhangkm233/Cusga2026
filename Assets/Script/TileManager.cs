using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// 操控地块的生成 挂载在GameManager物体上
/// 管理游戏内的TILE物体
/// </summary>
public class TileManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private GameObject tileParentGameobject; // 地块的父物体

    [SerializeField]
    private float tileSpaceing = 1.5f; // 地块间距

    public bool IsGenerated = false;

    public static TileManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        StartCoroutine(DelayedInit());
    }

    private System.Collections.IEnumerator DelayedInit() {
        // 等待MapManager初始化完成
        while (MapManager.Instance == null || MapManager.Instance.LandMap == null || MapManager.Instance.LandMap.Count == 0) {
            yield return null;
        }
        GenerateTiles();
    }

    /// <summary>
    /// 根据MapManger中的地图数据生成地块
    /// </summary>
    [ContextMenu("GenerateTiles")]
    public void GenerateTiles() {
        if (tilePrefab == null) {
            Debug.LogWarning("tilePrefab未设置");
            return;
        }

        // 清除现有地块（这里可以改成对象池？）
        GameObject[] existingTiles = GameObject.FindGameObjectsWithTag("TileGameobject");
        foreach (GameObject tile in existingTiles) {
            Destroy(tile);
        }

        // 生成地块
        for (int i = 0;i < MapManager.Instance.LandMap.Count;i++) {
            for (int j = 0;j < MapManager.Instance.LandMap[i].Count;j++) {
                //实例化地块 与0，0，0位置实现对称平铺
                //TODO：这里的位置需要细调 以适应地块大小和间距
                //UPDATE： 调完了、
                GameObject tileToinstantiate = null;
                try
                {
                    //tileToinstantiate = DatabaseManager.Instance.GetLandSOByType(MapManager.Instance.LandMap[i][j].LandType).LandPrefab;
                    
                    tileToinstantiate = tilePrefab;
                } catch (System.Exception e)
                {
                    Debug.LogWarning($"TileManager:无法获取地块预制体，使用默认预制体。错误信息：{e.Message}");
                    tileToinstantiate = tilePrefab;
                }
                GameObject newTile = Instantiate(tileToinstantiate,
                    new Vector3(
                        (j - (MapManager.Instance.ColCount - 1) / 2f) * tileSpaceing,
                        0.5f,
                        (i - (MapManager.Instance.RowCount - 1) / 2f) * tileSpaceing
                    ),Quaternion.identity);
                newTile.name = $"Tile_{i}_{j}";
                //设置父物体
                if (tileParentGameobject == null) {
                    tileParentGameobject = new GameObject("TileParent");
                }
                newTile.transform.parent = tileParentGameobject.transform;
                TileController tileController = newTile.GetComponent<TileController>();
                if (tileController != null) {
                    tileController.tileRow = i;
                    tileController.tileCol = j;
                    tileController.land = MapManager.Instance.LandMap[i][j];
                    
                    newTile.GetComponent<TileDataManager>().UpdateDataDisplay();
                } else {
                    tileController = newTile.AddComponent<TileController>();
                    BoxCollider boxCollider = newTile.AddComponent<BoxCollider>();
                    boxCollider.size = new Vector3(5f,2f,5f);
                    //这里有bug，模型的原点不在中心
                    //TODO：以后换模型的时候注意调整原点位置
                    newTile.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
                    tileController.tileRow = i;
                    tileController.tileCol = j;
                    tileController.land = MapManager.Instance.LandMap[i][j];
                }
            }
        }
        Debug.Log("TileManager:地块生成完成");
        IsGenerated = true;
        // 是不是还需要调整摄像机位置以适应新地图？ 有待商榷，回头再弄
    }

    public Transform GetTileTransform(int row,int col) {
        string tileName = $"Tile_{row}_{col}";
        GameObject tile = GameObject.Find(tileName);
        if (tile != null) {
            return tile.transform;
        } else {
            Debug.LogWarning($"未找到地块 {tileName}");
            return null;
        }
    }

    public Vector3 GetTilePosition(int row,int col) {
        Transform tileTransform = GetTileTransform(row,col);
        if (tileTransform != null) {
            return tileTransform.position;
        } else {
            return Vector3.zero;
        }
    }

}
