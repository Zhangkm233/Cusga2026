using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //用于管理逻辑层Land对象的生成和移动

    public List<List<Land>> LandMap = new List<List<Land>>();//储存所有land数据 
    public static MapManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void InitializeMap(int rows,int cols) {
        // 初始化地图
        LandMap.Clear();
        for (int i = 0;i < rows;i++) {
            List<Land> row = new List<Land>();
            for (int j = 0;j < cols;j++) {
                Land newLand = new ForestLand();
                row.Add(newLand);
            }
            LandMap.Add(row);
        }
        Debug.Log("地图已初始化");
    }

    public void InitiallizeMap() {
        //一开始，玩家拥有一块3x4的地块，其中包含4块山丘，5块平原，3块森林
        LandMap.Clear();
        List<Land> row1 = new List<Land> { new HillLand(0,0),new HillLand(0,1),new HillLand(0,2),new HillLand(0,3) };
        List<Land> row2 = new List<Land> { new PlainLand(),new PlainLand(),new PlainLand(),new PlainLand() };
        List<Land> row3 = new List<Land> { new PlainLand(),new ForestLand(),new ForestLand(),new ForestLand() };
        LandMap.Add(row1);
        LandMap.Add(row2);
        LandMap.Add(row3);
    }
}
