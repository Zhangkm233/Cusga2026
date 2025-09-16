using UnityEngine;

public abstract class Animal : MonoBehaviour
{
    private string animalName; //动物名称
    private int health; //生命值
    private Card drop; //掉落卡片
    private int dropNum; //掉落数量
    private int preferLand; //偏好地形类型
    private int mapRow; //所在地图行
    private int mapCol; //所在地图列

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

    public void MoveMent() {
        //动物移动
        //移动到自身所在格与相邻四格内所有空旷的格子中，最接近自己偏好地势数值的地块
        Debug.Log("移动");

    }
}
