using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //���ڹ����߼���Land��animal��������ɺ��ƶ�

    public List<List<Land>> LandMap = new List<List<Land>>();//��������land���� 
    public List<List<Animal>> AnimalMap = new List<List<Animal>>();//��������animal����

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
        Debug.Log("��ͼ�ѳ�ʼ��");
    }

    public void InitiallizeLandMap() {
        //һ��ʼ�����ӵ��һ��3x4�ĵؿ飬���а���4��ɽ��5��ƽԭ��3��ɭ��
        LandMap.Clear();
        AnimalMap.Clear();
        
        List<Land> row1 = new List<Land> { new HillLand(0,0),new HillLand(0,1),new HillLand(0,2),new HillLand(0,3) };
        List<Land> row2 = new List<Land> { new PlainLand(1,0),new PlainLand(1,1),new PlainLand(1,2),new PlainLand(1,3) };
        List<Land> row3 = new List<Land> { new PlainLand(2,0),new ForestLand(2,1),new ForestLand(2,2),new ForestLand(2,3) };
        LandMap.Add(row1);
        LandMap.Add(row2);
        LandMap.Add(row3);
        
        // 初始化动物地图
        for (int i = 0; i < 3; i++) {
            List<Animal> animalRow = new List<Animal> { null, null, null, null };
            AnimalMap.Add(animalRow);
        }
        
        Debug.Log("��ͼ�ѳ�ʼ��");
    }

    public void StateCheck() {
        //���ÿ��ص�״̬
        foreach (var row in LandMap) {
            foreach (var land in row) {
                land.PassiveEffect();
            }
        }
    }

    public void AddAnimalToMap(Animal animal,int row,int col) {
        //���Ӷ��ﵽ��ͼ
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("λ�ó�����ͼ��Χ");
            return;
        }
        if (IsPositionBeenOccupiedByAnimal(row,col)) {
            Debug.LogError("λ���ѱ�ռ��");
            return;
        }
        AnimalMap[row][col] = animal;
        animal.MapRow = row;
        animal.MapCol = col;
        Debug.Log($"����{animal.AnimalName}�����ӵ���ͼλ��({row},{col})");
    }

    public bool IsPositionBeenOccupiedByAnimal(int row,int col) {
        //���λ���Ƿ�ռ��
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("λ�ó�����ͼ��Χ");
            return true;
        }
        return AnimalMap[row][col] != null;
    }

    public void RemoveAnimalFromMap(int row,int col) {
        //�ӵ�ͼ�Ƴ�����
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("λ�ó�����ͼ��Χ");
            return;
        }
        if (AnimalMap[row][col] == null) {
            Debug.LogError("��λ��û�ж���");
            return;
        }
        Animal removedAnimal = AnimalMap[row][col];
        AnimalMap[row][col] = null;
        Debug.Log($"����{removedAnimal.AnimalName}�Ѵӵ�ͼλ��({row},{col})�Ƴ�");
    }

    public void MoveAnimalToMap(Animal animal,int row,int col) {
        if (row < 0 || row >= AnimalMap.Count || col < 0 || col >= AnimalMap[0].Count) {
            Debug.LogError("λ�ó�����ͼ��Χ");
            return;
        }
        if (IsPositionBeenOccupiedByAnimal(row,col)) {
            Debug.LogError("λ���ѱ�ռ��");
            return;
        }
        RemoveAnimalFromMap(animal.MapRow,animal.MapCol);
        AddAnimalToMap(animal,row,col);
        Debug.Log($"����{animal.AnimalName}���ƶ�����ͼλ��({row},{col})");
    }

    public int GetAmountOfLandType(LandType landType) {
        //��ȡĳ�ֵ��ε�����
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
        //�����ؿ����
        AddAnimalToMap(new Animal(animalType),land.MapRow,land.MapCol);
    }

    public int CountAdjacentLandType(int row,int col,LandType landType) {
        if (row < 0 || row >= LandMap.Count || col < 0 || col >= LandMap[0].Count) {
            Debug.LogError("λ�ó�����ͼ��Χ");
            return 0;
        }
        int count = 0;
        //����Ϸ�
        if (row > 0 && LandMap[row - 1][col] != null && LandMap[row - 1][col].LandType == landType) {
            count++;
        }
        //����·�
        if (row < LandMap.Count - 1 && LandMap[row + 1][col] != null && LandMap[row + 1][col].LandType == landType) {
            count++;
        }
        //�����
        if (col > 0 && LandMap[row][col - 1] != null && LandMap[row][col - 1].LandType == landType) {
            count++;
        }
        //����ҷ�
        if (col < LandMap[0].Count - 1 && LandMap[row][col + 1] != null && LandMap[row][col + 1].LandType == landType) {
            count++;
        }
        return count;
    }

    public void EnergyPhase() {
        //���ܽ׶�
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

    public void TransformRandomLand(LandType landType) {
        //�����һ���ת��Ϊָ������ ���ų��Ѿ��Ǹõ��εĵؿ�
        List<Land> candidateLands = new List<Land>();
        foreach (var row in LandMap) {
            foreach (var land in row) {
                if (land.LandType != landType) {
                    candidateLands.Add(land);
                }
            }
        }
        if (candidateLands.Count == 0) {
            Debug.LogWarning("û�п�ת���ĵؿ�");
            return;
        }
        int randomIndex = Random.Range(0,candidateLands.Count);
        Land landToTransform = candidateLands[randomIndex];
        landToTransform.ChangeLandType(landType);
    }

    public Land GetLandAt(int row,int col) {
        if (row < 0 || row >= LandMap.Count || col < 0 || col >= LandMap[0].Count) {
            Debug.LogError("λ�ó�����ͼ��Χ");
            return null;
        }
        return LandMap[row][col];
    }
}