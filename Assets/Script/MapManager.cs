using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapManager : MonoBehaviour
{

    public List<List<Land>> LandMap = new List<List<Land>>();
    public List<List<Animal>> AnimalMap = new List<List<Animal>>();
    public UnityEvent<int,int> OnLandChanged;

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
        // ��ʼ����ͼ
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
        //3山4平2森3废墟
        LandMap.Clear();
        AnimalMap.Clear();

        List<Land> row1 = new List<Land> { new HillLand(0,0),new HillLand(0,1),new HillLand(0,2),new RuinLand(0,3) };
        List<Land> row2 = new List<Land> { new PlainLand(1,0),new PlainLand(1,1),new PlainLand(1,2),new PlainLand(1,3) };
        List<Land> row3 = new List<Land> { new RuinLand(2,0),new ForestLand(2,1),new ForestLand(2,2),new RuinLand(2,3) };
        LandMap.Add(row1);
        LandMap.Add(row2);
        LandMap.Add(row3);

        // 初始化动物地图
        for (int i = 0;i < 3;i++) {
            List<Animal> animalRow = new List<Animal> { null,null,null,null };
            AnimalMap.Add(animalRow);
        }

        Debug.Log("地图初始化完成");
    }

    public void StateCheck() {
        //检查所有地形是否触发被动效果
        foreach (var row in LandMap) {
            foreach (var land in row) {
                land.PassiveEffect();
            }
        }
    }

    public void AddAnimalToMap(Animal animal,int row,int col) {
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("超出地图范围");
            return;
        }
        if (IsPositionBeenOccupiedByAnimal(row,col)) {
            Debug.LogError("已有动物");
            return;
        }
        AnimalMap[row][col] = animal;
        animal.MapRow = row;
        animal.MapCol = col;
        Debug.Log($"生物{animal.AnimalName}生成在({row},{col})");
    }

    public bool IsPositionBeenOccupiedByAnimal(int row,int col) {
        //检查目标位置是否已有动物
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("超出地图范围");
            return true;
        }
        return AnimalMap[row][col] != null;
    }

    public void RemoveAnimalFromMap(int row,int col) {
        //从地图上移除动物
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("超出地图范围");
            return;
        }
        if (AnimalMap[row][col] == null) {
            Debug.LogError("该位置没有动物");
            return;
        }
        Animal removedAnimal = AnimalMap[row][col];
        AnimalMap[row][col] = null;
        Debug.Log($"生物{removedAnimal.AnimalName}已从({row},{col})上移除");
    }

    public void MoveAnimalToMap(Animal animal,int row,int col) {
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("超出地图范围");
            return;
        }
        if (IsPositionBeenOccupiedByAnimal(row,col)) {
            Debug.LogError("已有生物");
            return;
        }
        RemoveAnimalFromMap(animal.MapRow,animal.MapCol);
        AddAnimalToMap(animal,row,col);
        Debug.Log($"生物{animal.AnimalName}已移动至({row},{col})");
    }

    public int GetAmountOfLandType(LandType landType) {
        //获得给定地形类型的数量
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
            Debug.LogError("�õؿ���û�ж���޷�����˺�");
        }
    }

    public void AddAnimalToLand(AnimalType animalType,Land land) {
        //添加动物到地块
        AddAnimalToMap(new Animal(animalType),land.MapRow,land.MapCol);
    }

    public int CountAdjacentLandType(int row,int col,LandType landType) {
        if (row < 0 || row >= LandMap.Count || col < 0 || col >= LandMap[0].Count) {
            Debug.LogError("超出地图范围");
            return 0;
        }
        int count = 0;
        if (row > 0 && LandMap[row - 1][col] != null && LandMap[row - 1][col].LandType == landType) {
            count++;
        }
        if (row < LandMap.Count - 1 && LandMap[row + 1][col] != null && LandMap[row + 1][col].LandType == landType) {
            count++;
        }
        if (col > 0 && LandMap[row][col - 1] != null && LandMap[row][col - 1].LandType == landType) {
            count++;
        }
        if (col < LandMap[0].Count - 1 && LandMap[row][col + 1] != null && LandMap[row][col + 1].LandType == landType) {
            count++;
        }
        return count;
    }

    public void EnergyPhase() {
        foreach (var row in LandMap) {
            foreach (var land in row) {
                land.AddEnergy(1);
                land.PassiveEffect();
            }
        }
    }

    public void AnimalPhase() {
        if (AnimalMap.Count == 0) {
            Debug.Log("没有动物");
            return;
        }
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


    public void TransformRandomLand(LandType landType) {
        //随机将一个非该类型的地块转变为该类型
        List<Land> candidateLands = new List<Land>();
        foreach (var row in LandMap) {
            foreach (var land in row) {
                if (land.LandType != landType) {
                    candidateLands.Add(land);
                }
            }
        }
        if (candidateLands.Count == 0) {
            Debug.LogWarning("没有合法地形");
            return;
        }
        int randomIndex = Random.Range(0,candidateLands.Count);
        Land landToTransform = candidateLands[randomIndex];
        TransformLandAt(landToTransform.MapRow,landToTransform.MapCol,landType);
    }

    public void TransformLandAt(int row,int col,LandType landType) {
        //将指定位置的地块转变为该类型
        if (row < 0 || row >= LandMap.Count || col < 0 || col >= LandMap[0].Count) {
            Debug.LogError("超出地图范围");
            return;
        }
        Land landToTransform = LandMap[row][col];
        if (landType == LandType.RUIN) {
            if (landToTransform.TryUseSoild()) {
                landToTransform.ChangeLandType(landType);
            }
        } else {
            landToTransform.ChangeLandType(landType);
        }
        OnLandChanged.Invoke(row,col);
    }

    public Land GetLandAt(int row,int col) {
        //获取指定位置的地块
        if (row < 0 || row >= LandMap.Count || col < 0 || col >= LandMap[0].Count) {
            Debug.LogError("超出地图范围");
            return null;
        }
        return LandMap[row][col];
    }

    public bool IsPlaceLegal(int row,int col) {
        //检查目标位置是否在地图范围内且有地形
        if (row < 0 || row >= LandMap.Count || col < 0 || col >= LandMap[0].Count) {
            Debug.LogError("超出地图范围");
            return false;
        }
        return LandMap[row][col] != null;
    }

    public Animal GetAnimalAt(int row,int col) {
        //获取指定位置的动物
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("超出地图范围");
            return null;
        }
        return AnimalMap[row][col];
    }
}