using UnityEngine;


/// <summary>
/// 操控地块的生成 挂载在GameManager物体上
/// </summary>
public class TileManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private GameObject tileParentGameobject; // 地块的父物体
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
        //GenerateTiles();
    }

    /// <summary>
    /// 根据MapManger中的地图数据生成地块
    /// </summary>
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
                GameObject newTile = Instantiate(tilePrefab,new Vector3(j - (MapManager.Instance.ColCount - 1) / 3f,0.5f,i - (MapManager.Instance.RowCount - 1) / 3f),Quaternion.identity);
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
                    Debug.LogError("TilePrefab缺少TileController组件");
                }
            }
        }

        // 是不是还需要调整摄像机位置以适应新地图？ 有待商榷，回头再弄

    }
}
