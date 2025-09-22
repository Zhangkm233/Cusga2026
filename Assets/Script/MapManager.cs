using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //用于管理逻辑层Land和animal对象的生成和移动

    public List<List<Land>> LandMap = new List<List<Land>>();//储存所有land数据 
    public List<List<Animal>> AnimalMap = new List<List<Animal>>();//储存所有animal数据

    public static MapManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void InitializeLandMap(int rows,int cols) {
        // 初始化地图
        LandMap.Clear();
        for (int i = 0;i < rows;i++) {
            List<Land> row = new List<Land>();
            for (int j = 0;j < cols;j++) {
                Land newLand = new ForestLand(rows,cols);
                row.Add(newLand);
            }
            LandMap.Add(row);
        }
        Debug.Log("地图已初始化");
    }

    public void InitiallizeLandMap() {
        //一开始，玩家拥有一块3x4的地块，其中包含4块山丘，5块平原，3块森林
        LandMap.Clear();
        List<Land> row1 = new List<Land> { new HillLand(0,0),new HillLand(0,1),new HillLand(0,2),new HillLand(0,3) };
        List<Land> row2 = new List<Land> { new PlainLand(1,0),new PlainLand(1,1),new PlainLand(1,2),new PlainLand(1,3) };
        List<Land> row3 = new List<Land> { new PlainLand(2,0),new ForestLand(2,1),new ForestLand(2,2),new ForestLand(2,3) };
        LandMap.Add(row1);
        LandMap.Add(row2);
        LandMap.Add(row3);
        Debug.Log("地图已初始化");
    }

    public void StateCheck() {
        //检查每块地的状态
        foreach (var row in LandMap) {
            foreach (var land in row) {
                land.PassiveEffect();
            }
        }
    }

    public void AddAnimalToMap(Animal animal,int row,int col) {
        //添加动物到地图
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("位置超出地图范围");
            return;
        }
        if (IsPositionBeenOccupiedByAnimal(row,col)) {
            Debug.LogError("位置已被占用");
            return;
        }
        AnimalMap[row][col] = animal;
        animal.MapRow = row;
        animal.MapCol = col;
        Debug.Log($"动物{animal.AnimalName}已添加到地图位置({row},{col})");
    }

    public bool IsPositionBeenOccupiedByAnimal(int row,int col) {
        //检查位置是否被占用
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("位置超出地图范围");
            return true;
        }
        return AnimalMap[row][col] != null;
    }

    public void RemoveAnimalFromMap(int row,int col) {
        //从地图移除动物
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("位置超出地图范围");
            return;
        }
        if (AnimalMap[row][col] == null) {
            Debug.LogError("该位置没有动物");
            return;
        }
        Animal removedAnimal = AnimalMap[row][col];
        AnimalMap[row][col] = null;
        Debug.Log($"动物{removedAnimal.AnimalName}已从地图位置({row},{col})移除");
    }

    public void MoveAnimalToMap(Animal animal,int row,int col) {
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("位置超出地图范围");
            return;
        }
        if (IsPositionBeenOccupiedByAnimal(row,col)) {
            Debug.LogError("位置已被占用");
            return;
        }
        RemoveAnimalFromMap(animal.MapRow,animal.MapCol);
        AddAnimalToMap(animal,row,col);
        Debug.Log($"动物{animal.AnimalName}已移动到地图位置({row},{col})");
    }

    public int GetAmountOfLandType(LandType landType) {
        //获取某种地形的数量
        int count = 0;
        foreach (var row in LandMap) {
            foreach (var land in row) {
                if (land.LandType == landType) {
                    count++;
                }
            }
        }
        return count;
    }

    public void DealDamageTo(Animal animal,int damage) {
        animal.TakeDamage(damage);
    }

    public void DealDamageTo(Land land,int damage) {
        if (AnimalMap[land.MapRow][land.MapCol] != null) {
            DealDamageTo(AnimalMap[land.MapRow][land.MapCol],(damage));
        } else {
            Debug.LogError("该地块上没有动物，无法造成伤害");
        }
    }

    public void AddAnimalToLand(AnimalType animalType,Land land) {
        //留给地块调用
        AddAnimalToMap(new Animal(animalType),land.MapRow,land.MapCol);
    }

    public int CountAdjacentLandType(int row,int col,LandType landType) {
        if (row < 0 || row >= LandMap.Count || col < 0 || col >= LandMap[0].Count) {
            Debug.LogError("位置超出地图范围");
            return 0;
        }
        int count = 0;
        //检查上方
        if (row > 0 && LandMap[row - 1][col] != null && LandMap[row - 1][col].LandType == landType) {
            count++;
        }
        //检查下方
        if (row < LandMap.Count - 1 && LandMap[row + 1][col] != null && LandMap[row + 1][col].LandType == landType) {
            count++;
        }
        //检查左方
        if (col > 0 && LandMap[row][col - 1] != null && LandMap[row][col - 1].LandType == landType) {
            count++;
        }
        //检查右方
        if (col < LandMap[0].Count - 1 && LandMap[row][col + 1] != null && LandMap[row][col + 1].LandType == landType) {
            count++;
        }
        return count;
    }

    public void EnergyPhase() {
        //充能阶段
        foreach (var row in LandMap) {
            foreach (var land in row) {
                land.AddEnergy(1);
                land.PassiveEffect();
            }
        }
    }

    public void AnimalPhase() {
        foreach (var row in AnimalMap) {
            foreach (var animal in row) {
                animal.MoveToPreferLand();
            }
        }
    }

    public void ExtraEffectPhase() {
        foreach (var row in LandMap) {
            foreach (var land in row) {
                land.ExtraEffect();
            }
        }
    }
}
