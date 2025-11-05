using UnityEngine;

public enum AnimalType
{
    GOBLIN, //哥布林
    BEAR, //熊
    BOAR, //猪
}

public class Animal : IDamageable
{
    private string animalName; //动物名称
    private int health; //生命值
    private Card drop; //掉落卡片
    private int dropNum; //掉落数量
    private int preferLand; //偏好地形类型
    private int mapRow; //所在地图行
    private int mapCol; //所在地图列
    private AnimalType animalType; //动物类型

    public AnimalType AnimalType {
        get { return animalType; }
        set { animalType = value; }
    }
    public int PreferLand {
        get { return preferLand; }
        set { preferLand = value; }
    }
    public int DropNum {
        get { return dropNum; }
        set { dropNum = value; }
    }
    public int Health {
        get { return health; }
        set { health = value; }
    }
    public Card Drop {
        get { return drop; }
        set { drop = value; }
    }
    public string AnimalName {
        get { return animalName; }
        set { animalName = value; }
    }
    public int MapRow {
        get { return mapRow; }
        set { mapRow = value; }
    }
    public int MapCol {
        get { return mapCol; }
        set { mapCol = value; }
    }

    public Animal(AnimalType animalType) {
        this.animalType = animalType;
        switch (animalType) {
            case AnimalType.GOBLIN:
                animalName = "哥布林";
                health = 2;
                drop = new MaterialCard(MaterialType.MEAT);
                dropNum = 1;
                preferLand = 30;
                break;
            case AnimalType.BEAR:
                animalName = "熊";
                health = 5;
                drop = new MaterialCard(MaterialType.MEAT);
                dropNum = 3;
                preferLand = 20;
                break;
            case AnimalType.BOAR:
                animalName = "猪";
                health = 3;
                drop = new MaterialCard(MaterialType.MEAT);
                dropNum = 4;
                preferLand = 15; //偏好丘陵
                break;
            default:
                Debug.LogError("未知动物类型");
                break;
        }
    }
    public void MoveToPreferLand() {
        //动物移动
        //移动到自身所在格与相邻四格内所有空旷的格子中，最接近自己偏好地势数值的地块
        Debug.Log("移动");
        int bestRow = mapRow;
        int bestCol = mapCol;
        int bestPreferenceDiff = Mathf.Abs(MapManager.Instance.LandMap[mapRow][mapCol].Preference - preferLand);
        if (mapRow > 0) { //上
            if (MapManager.Instance.LandMap[mapRow - 1][mapCol] != null && MapManager.Instance.AnimalMap[mapRow - 1][MapCol] == null ) {
                if (Mathf.Abs(MapManager.Instance.LandMap[mapRow - 1][mapCol].Preference - preferLand) < bestPreferenceDiff) {
                    bestRow = mapRow - 1;
                    bestCol = mapCol;
                    bestPreferenceDiff = Mathf.Abs(MapManager.Instance.LandMap[mapRow - 1][mapCol].Preference - preferLand);
                }
            }
        }
        if (mapRow < MapManager.Instance.LandMap.Count - 1) { //下
            if (MapManager.Instance.LandMap[mapRow + 1][mapCol] != null && MapManager.Instance.AnimalMap[mapRow + 1][MapCol] == null) {
                if (Mathf.Abs(MapManager.Instance.LandMap[mapRow + 1][mapCol].Preference - preferLand) < bestPreferenceDiff) {
                    bestRow = mapRow + 1;
                    bestCol = mapCol;
                    bestPreferenceDiff = Mathf.Abs(MapManager.Instance.LandMap[mapRow + 1][mapCol].Preference - preferLand);
                }
            }
        }
        if (mapCol > 0) { //左
            if (MapManager.Instance.LandMap[mapRow][mapCol - 1] != null && MapManager.Instance.AnimalMap[mapRow][MapCol - 1] == null) {
                if (Mathf.Abs(MapManager.Instance.LandMap[mapRow][mapCol - 1].Preference - preferLand) < bestPreferenceDiff) {
                    bestRow = mapRow;
                    bestCol = mapCol - 1;
                    bestPreferenceDiff = Mathf.Abs(MapManager.Instance.LandMap[mapRow][mapCol - 1].Preference - preferLand);
                }
            }
        }
        if (mapCol < MapManager.Instance.LandMap[0].Count - 1) { //右
            if (MapManager.Instance.LandMap[mapRow][mapCol + 1] != null && MapManager.Instance.AnimalMap[mapRow][MapCol + 1] == null) {
                if (Mathf.Abs(MapManager.Instance.LandMap[mapRow][mapCol + 1].Preference - preferLand) < bestPreferenceDiff) {
                    bestRow = mapRow;
                    bestCol = mapCol + 1;
                    bestPreferenceDiff = Mathf.Abs(MapManager.Instance.LandMap[mapRow][mapCol + 1].Preference - preferLand);
                }
            }
        }

        //移动
        if (bestRow != mapRow || bestCol != mapCol) {
            MapManager.Instance.MoveAnimalToMap(this,bestRow,bestCol);
        }
    }

    public void TakeDamage(int damage) {
        //动物受伤
        health -= damage;
        Debug.Log($"动物{animalName}受到了{damage}点伤害，当前生命值为{health}");
        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        //动物死亡
        Debug.Log($"动物{animalName}死亡");
        //掉落卡片
        for (int i = 0;i < dropNum;i++) {
            DeckManager.Instance.AddCardToDeck(drop);
        }
        //从地图移除
        MapManager.Instance.RemoveAnimalFromMap(mapRow,mapCol);
        //销毁对应的GameObject
    }
}
