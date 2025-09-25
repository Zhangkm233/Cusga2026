using System.ComponentModel;
using UnityEngine;

public enum LandType
{
    HILL, //山丘
    PLAIN, //平原
    FOREST, //森林
    MOUNTAIN, //山脉
    JUNGLE, //密林
    WHEATLAND, //麦田
    CABIN, //木屋
    THATCH, //茅屋
    TOWN, //小镇
    WAREHOUSE, //仓库
    TOWER, //塔
    WINDMILL //风车
}

public abstract class Land
{
    private LandType landType; //地形类型
    private int energyCounter = 0; //计数器，表示该地形上有多少个充能
    private int requiredEnergy; //触发被动效果需要的充能数量
    public MaterialType storageCardType = MaterialType.NULL; //已存储的卡片类别
    public int storageCardNum = 0; //存储的卡片数量
    private int preference = 0; //偏好地形数值
    private int soild = 0; //坚固值 可以抵挡一次攻击或者天灾效果
    private int hunterarea = 0;//猎圈值 额外收益的概率增加一倍
    private static int maxSoild = 1; //最大坚固值
    private static int maxHunterarea = 2; //最大猎圈值
    private int mapRow; //地形所在地图行
    private int mapCol; //地形所在地图列

    public delegate void LandChangeEventHandler(Land oldLand,Land newLand);
    public event LandChangeEventHandler OnLandChanged; //地形变化事件
    
    public int RequiredEnergy {
        get { return requiredEnergy; }
        set { requiredEnergy = value; }
    }
    public LandType LandType {
        get { return landType; }
        set { landType = value; }
    }
    public int Soild {
        get { return soild; }
        set { soild = value; }
    }
    public int Hunterarea {
        get { return hunterarea; }
        set { hunterarea = value; }
    }
    public int MapRow {
        get { return mapRow; }
        set { mapRow = value; }
    }
    public int MapCol {
        get { return mapCol; }
        set { mapCol = value; }
    }

    public int EnergyCounter {
        get { return energyCounter; }
        set { energyCounter = value; }
    }
    public int Preference {
        get { return preference; }
        set { preference = value; }
    }

    public bool IsRandomEventTriggered(int probability) {
        int rand = Random.Range(1,101); // 生成1到100的随机数
        return rand <= probability; // 如果随机数小于等于概率值，则事件触发
    }

    public void AddEnergy(int energy) {
        energyCounter += energy; // 增加充能
        PassiveEffect();
        Debug.Log($"地形 {landType} 充能增加 {energy}，当前充能：{energyCounter}");
    }

    public void SpawnAnimal(AnimalType animalType) {
        MapManager.Instance.AddAnimalToLand(animalType,this);
    }

    public abstract void PassiveEffect(); //被动效果
    public abstract void MaterialEffect(MaterialCard materialCard);
    public abstract void ExtraEffect(); //额外效果

    public void StorageIn(MaterialType materialType) {
        if (storageCardType == materialType) {
            storageCardNum += 1; // 存储的卡片数量增加1
        } else {
            storageCardType = materialType; // 存储的卡片类型为材料
            storageCardNum = 1; // 存储的卡片数量设置为1
        }
    }

    public void AddSoild(int value) {
        soild += value;
        if (soild > maxSoild) {
            soild = maxSoild; // 坚固值不能超过最大值
        }
    }
    public void AddHunterarea(int value) {
        hunterarea += value;
        if (hunterarea > maxHunterarea) {
            hunterarea = maxHunterarea; // 猎圈值不能超过最大值
        }
    }

    public void AddCard(Card card,int num) {
        DeckManager.Instance.AddCardToDeckFromLand(card,num,this);
    }
    public void AddExtraDraw(CardType cardType,int num) {
        for (int i = 0;i < num;i++) {
            DeckManager.Instance.extraCertainCardType.Add(cardType);
        }
    }

    public void AddExtraDraw(CardType cardType) {
        DeckManager.Instance.extraCertainCardType.Add(cardType);
    }

    public void AddExtraDraw(int cardId,int num) {
        for (int i = 0;i < num;i++) {
            DeckManager.Instance.extraCertainCardId.Add(cardId);
        }
    }

    public void AddExtraDraw(int cardId) {
        DeckManager.Instance.extraCertainCardId.Add(cardId);
    }


    public void ChangeLandType(LandType landType) {
        //这里是转变地形的函数 如果想解耦合，还需要改一改到游戏里面的调用，应该UI层面需要全部重写
        //需要在逻辑层里面添加存储 然后再显示到游戏/UE层

        MapManager.Instance.LandMap[mapRow][mapCol] = null;
        Land newLand = null;
        switch (landType) {
            case LandType.HILL:
                newLand = new HillLand(mapRow,mapCol);
                break;
            case LandType.PLAIN:
                newLand = new PlainLand(mapRow,mapCol);
                break;
            case LandType.FOREST:
                newLand = new ForestLand(mapRow,mapCol);
                break;
            case LandType.THATCH:
                newLand = new ThatchLand(mapRow,mapCol);
                break;
            case LandType.CABIN:
                newLand = new CabinLand(mapRow,mapCol);
                break;
            case LandType.MOUNTAIN:
                newLand = new MountainLand(mapRow,mapCol);
                break;
            case LandType.TOWN:
                newLand = new TownLand(mapRow,mapCol);
                break;
            case LandType.TOWER:
                newLand = new TowerLand(mapRow,mapCol);
                break;
            case LandType.WAREHOUSE:
                newLand = new WarehouseLand(mapRow,mapCol);
                break;
            case LandType.WINDMILL:
                newLand = new WindmillLand(mapRow,mapCol);
                break;
            case LandType.JUNGLE:
                newLand = new JungleLand(mapRow,mapCol);
                break;
            case LandType.WHEATLAND:
                newLand = new WheatLand(mapRow,mapCol);
                break;
        }
        //继承所有特殊属性（坚固和猎圈之类的）（充能不继承）
        newLand.soild = this.soild;
        newLand.hunterarea = this.hunterarea;
        MapManager.Instance.LandMap[mapRow][mapCol] = newLand;
        //this.gameObject.AddComponent(newLand.GetType());
        //这里我不知道写的对不对，尚待商榷
        //Destroy(this);
        //这里需要更改游戏内对应的gameobject

        OnLandChanged.Invoke(this,newLand);
    }
}

public class HillLand : Land 
{
    // 山丘地形
    public HillLand(int row, int col) {
        LandType = LandType.HILL;
        RequiredEnergy = 4;
        Preference = 20;
        MapRow = row;
        MapCol = col;
    }

    public override void PassiveEffect() {
        // 山丘地形的被动效果 4时产1石
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("山丘地形被动效果触发");
            AddCard(new MaterialCard(MaterialType.STONE),1); //添加一个石头材料卡到牌库
            EnergyCounter = 0; // 重置计数器
        }
    }

    public override void MaterialEffect(MaterialCard materialCard) {
        switch(materialCard.materialType) {
            case MaterialType.WOOD:
                //4升木屋坚固
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 4 && storageCardType == MaterialType.WOOD) {
                    AddSoild(1);
                    ChangeLandType(LandType.CABIN);
                }
                break;
            case MaterialType.HAY:
                //产2木材
                AddCard(new MaterialCard(MaterialType.WOOD),2);
                break;
            case MaterialType.STONE:
                //1升山脉
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 1 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.MOUNTAIN);
                }
                break;
            case MaterialType.MEAT:
                //产1熊
                SpawnAnimal(AnimalType.BEAR);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //这里还没写 应该是20%产1石
        if(IsRandomEventTriggered(20)) {
            Debug.Log("山丘地形额外效果触发");
            AddCard(new MaterialCard(MaterialType.STONE),1  ); //添加一个石头材料卡到牌库
        }
    }
}

public class PlainLand : Land
{
    // 平原地形
    public PlainLand(int row,int col) {
        LandType = LandType.PLAIN;
        RequiredEnergy = 3;
        Preference = 30;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // 平原地形的被动效果 3时产1草
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("平原地形被动效果触发");
            AddCard(new MaterialCard(MaterialType.HAY),1); //添加一个干草材料卡到牌库
            EnergyCounter = 0; // 重置计数器
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //3升木屋
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 3 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.CABIN);
                }
                break;
            case MaterialType.HAY:
                //4升茅屋
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 4 && storageCardType == MaterialType.HAY) {
                    ChangeLandType(LandType.THATCH);
                }
                break;
            case MaterialType.STONE:
                //2升麦田
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 2 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.WHEATLAND);
                }
                break;
            case MaterialType.MEAT:
                //2产1兔1追猎
                StorageIn(MaterialType.MEAT);
                if (storageCardNum >= 2 && storageCardType == MaterialType.MEAT) {
                    SpawnAnimal(AnimalType.RABBIT);
                    AddCard(new SkillCard(SkillType.STALK),1);
                    storageCardNum = 0;
                    storageCardType = MaterialType.NULL;
                }
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //10%产1兔
        if (IsRandomEventTriggered(10)) {
            Debug.Log("平原地形额外效果触发");
            SpawnAnimal(AnimalType.RABBIT);
        }
    }
}

public class ForestLand : Land
{
    // 森林地形
    public ForestLand(int row, int col) {
        LandType = LandType.FOREST;
        RequiredEnergy = 2;
        Preference = 35;
        MapRow = row;
        MapCol = col;
    }

    public override void PassiveEffect() {
        // 森林地形的被动效果 2时产1木
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("森林地形被动效果触发");
            AddCard(new MaterialCard(MaterialType.WOOD),1); //添加一个木材材料卡到牌库
            EnergyCounter = 0; // 重置计数器
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //2升密林
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 2 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.JUNGLE);
                }
                break;
            case MaterialType.HAY:
                //产1兔
                SpawnAnimal(AnimalType.RABBIT);
                break;
            case MaterialType.STONE:
                //产1矛
                AddCard(new WeaponCard(WeaponType.SPEAR),1);
                break;
            case MaterialType.MEAT:
                //产1追猎
                AddCard(new SkillCard(SkillType.STALK),1);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class MountainLand : Land
{
    public MountainLand(int row,int col) {
        LandType = LandType.MOUNTAIN;
        RequiredEnergy = 4;
        Preference = 21;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // 山脉地形的被动效果 4时产2石
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("山脉地形被动效果触发");
            AddCard(new MaterialCard(MaterialType.STONE),2);
            EnergyCounter = 0; // 重置计数器
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //得1坚固
                AddSoild(1);
                break;
            case MaterialType.HAY:
                //2升山丘产3抽
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 2 && storageCardType == MaterialType.HAY) {
                    ChangeLandType(LandType.HILL);
                    DeckManager.Instance.ExtraDrawNum += 3;
                }
                break;
            case MaterialType.STONE:
                //产3滚石
                AddCard(new WeaponCard(WeaponType.ROLLROCK),3);
                break;
            case MaterialType.MEAT:
                //得1猎圈
                AddHunterarea(1);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //每回合有“相邻四格内山脉数量”*10%的概率获得1充能
        if(IsRandomEventTriggered(10 * MapManager.Instance.CountAdjacentLandType(MapRow,MapCol,LandType.MOUNTAIN))) {
            Debug.Log("山脉地形额外效果触发");
            AddEnergy(1);
        }
    }
}


public class JungleLand : Land
{
    public JungleLand(int row,int col) {
        LandType = LandType.JUNGLE;
        RequiredEnergy = 1;
        Preference = 36;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        //1时产1木
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("密林地形被动效果触发");
            AddCard(new MaterialCard(MaterialType.WOOD),1); //添加一个木材材料卡到牌库
            EnergyCounter = 0; // 重置计数器
        }
    }

    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //2升木屋
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 2 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.CABIN);
                }
                break;
            case MaterialType.HAY:
                //2产1熊
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 2 && storageCardType == MaterialType.HAY) {
                    SpawnAnimal(AnimalType.BEAR);
                    storageCardNum = 0;
                    storageCardType = MaterialType.NULL;
                }
                break;
            case MaterialType.STONE:
                //3升小镇
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 3 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.TOWN);
                }
                break;
            case MaterialType.MEAT:
                //产抽2肉
                AddExtraDraw((int)MaterialType.MEAT,2);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class WheatLand : Land
{
    public WheatLand(int row,int col) {
        LandType = LandType.WHEATLAND;
        RequiredEnergy = 4;
        Preference = 15;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        //4时产2草
        if (EnergyCounter >= RequiredEnergy) {
            Debug.Log("麦田地形被动效果触发");
            AddCard(new MaterialCard(MaterialType.HAY),2); //添加2个干草材料卡到牌库
            EnergyCounter = 0; // 重置计数器
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //产1收割
                AddCard(new SkillCard(SkillType.HARVEST),1);
                break;
            case MaterialType.HAY:
                //2产抽3草
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 2 && storageCardType == MaterialType.HAY) {
                    AddExtraDraw((int)MaterialType.HAY,3);
                    storageCardNum = 0;
                    storageCardType = MaterialType.NULL;
                }
                break;
            case MaterialType.STONE:
                //得1坚固2充能
                AddSoild(1);
                AddEnergy(2);
                break;
            case MaterialType.MEAT:
                //得3充能
                AddEnergy(3);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //30%产1草
        if (IsRandomEventTriggered(30)) {
            Debug.Log("麦田地形额外效果触发");
            AddCard(new MaterialCard(MaterialType.HAY),1); //添加一个干草材料卡到牌库
        }
    }
}

public class CabinLand : Land
{
    public CabinLand(int row,int col) {
        LandType = LandType.CABIN;
        RequiredEnergy = 4;
        Preference = 10;
        MapRow = row;
        MapCol = col;
    }

    public override void PassiveEffect() {
        // 木屋地形的被动效果
        if (EnergyCounter >= RequiredEnergy) {
            //4时产1抽
            Debug.Log("木屋地形被动效果触发");
            AddExtraDraw(CardType.MATERIAL);
            EnergyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //产1猎圈
                AddHunterarea(1);
                break;
            case MaterialType.HAY:
                //产1收割
                AddCard(new SkillCard(SkillType.HARVEST),1);
                break;
            case MaterialType.STONE:
                //产3滚石
                AddCard(new WeaponCard(WeaponType.ROLLROCK),3);
                break;
            case MaterialType.MEAT:
                //4升小镇
                StorageIn(MaterialType.MEAT);
                if (storageCardNum >= 4 && storageCardType == MaterialType.MEAT) {
                    ChangeLandType(LandType.TOWN);
                }
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //5%产1抽资源
        if (IsRandomEventTriggered(5)) {
            Debug.Log("木屋地形额外效果触发");
            AddExtraDraw(CardType.MATERIAL);
        }
    }
}

public class ThatchLand : Land
{
    public ThatchLand(int row,int col) {
        LandType = LandType.THATCH;
        RequiredEnergy = 5;
        Preference = 11;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // 茅屋地形的被动效果
        if (EnergyCounter >= RequiredEnergy) {
            //5时产2木1石
            Debug.Log("茅屋地形被动效果触发");
            AddCard(new MaterialCard(MaterialType.WOOD),2);
            AddCard(new MaterialCard(MaterialType.STONE),1);
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //产2矛
                AddCard(new WeaponCard(WeaponType.SPEAR),2);
                break;
            case MaterialType.HAY:
                //产1猪
                SpawnAnimal(AnimalType.BOAR);
                break;
            case MaterialType.STONE:
                //产1加固抽1技
                AddSoild(1);
                AddExtraDraw(CardType.SKILL);
                break;
            case MaterialType.MEAT:
                //产2追猎
                AddCard(new SkillCard(SkillType.STALK),2);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //15%得1充
        if (IsRandomEventTriggered(15)) {
            Debug.Log("茅屋地形额外效果触发");
            AddEnergy(1);
        }
    }
}




public class TownLand : Land
{

    bool IsExtraEffectTriggered = false;

    public TownLand(int row,int col) {
        LandType = LandType.TOWN;
        RequiredEnergy = 6;
        Preference = 9;
        MapRow = row;
        MapCol = col;
        IsExtraEffectTriggered = false;
    }
    public override void PassiveEffect() {
        // 城镇地形的被动效果
        if (EnergyCounter >= RequiredEnergy) {
            //6时产3肉
            Debug.Log("城镇地形被动效果触发");
            AddCard(new MaterialCard(MaterialType.MEAT),3); //添加3个肉材料卡到牌库
            EnergyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //5升仓库
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 5 && storageCardType == MaterialType.WOOD) {
                    ChangeLandType(LandType.WAREHOUSE);
                }
                break;
            case MaterialType.HAY:
                //4升风车
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 4 && storageCardType == MaterialType.HAY) {
                    ChangeLandType(LandType.WINDMILL);
                }
                break;
            case MaterialType.STONE:
                //1升塔
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 1 && storageCardType == MaterialType.STONE) {
                    ChangeLandType(LandType.TOWER);
                }
                break;
            case MaterialType.MEAT:
                //得2矛
                AddCard(new WeaponCard(WeaponType.SPEAR),2);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //额外抽2技一次
        if (!IsExtraEffectTriggered) {
            AddExtraDraw(CardType.SKILL,2);
            IsExtraEffectTriggered = true;
        }
    }
}


public class WarehouseLand : Land
{
    public WarehouseLand(int row,int col) {
        LandType = LandType.WAREHOUSE;
        RequiredEnergy = 2;
        Preference = 0;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // 仓库地形的被动效果
        if (EnergyCounter >= RequiredEnergy) {
            //2时产1抽资源
            Debug.Log("仓库地形被动效果触发");
            AddExtraDraw(CardType.MATERIAL);
            EnergyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //产1抽干草
                AddExtraDraw((int)MaterialType.HAY,1);
                break;
            case MaterialType.HAY:
                //产1抽木头
                AddExtraDraw((int)MaterialType.WOOD,1);
                break;
            case MaterialType.STONE:
                //产1抽资源,得坚固
                AddExtraDraw(CardType.MATERIAL);
                AddSoild(1);
                break;
            case MaterialType.MEAT:
                //产1抽石头
                AddExtraDraw((int)MaterialType.STONE,1);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //8%产1抽资源
        if (IsRandomEventTriggered(8)) {
            Debug.Log("仓库地形额外效果触发");
            AddExtraDraw(CardType.MATERIAL);
        }
    }
}

public class TowerLand : Land
{
    public TowerLand(int row,int col) {
        LandType = LandType.TOWER;
        RequiredEnergy = 3;
        Preference = -1;
        MapRow = row;
        MapCol = col;
    }
    public override void PassiveEffect() {
        // 塔地形的被动效果
        if (EnergyCounter >= RequiredEnergy) {
            //3时产短弓
            Debug.Log("塔地形被动效果触发");
            AddCard(new WeaponCard(WeaponType.BOW),1);
            EnergyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //得短弓
                AddCard(new WeaponCard(WeaponType.BOW),1);
                break;
            case MaterialType.HAY:
                //得2加固
                AddSoild(2);
                break;
            case MaterialType.STONE:
                //得2短弓2矛
                AddCard(new WeaponCard(WeaponType.BOW),2);
                AddCard(new WeaponCard(WeaponType.SPEAR),2);
                break;
            case MaterialType.MEAT:
                //得抽2技
                AddExtraDraw(CardType.SKILL,2);
                break;
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //造伤最小值+1 (这个效果在战斗系统里实现)
    }
}



public class WindmillLand : Land
{
    public WindmillLand(int row,int col) {
        LandType = LandType.WINDMILL;
        RequiredEnergy = 4;
        Preference = 24;
        MapRow = row;
        MapCol = col;

    }
    public override void PassiveEffect() {
        if (EnergyCounter >= RequiredEnergy) {
            //4时产1收割
            Debug.Log("风车地形被动效果触发");
            AddCard(new SkillCard(SkillType.HARVEST),1);
            EnergyCounter = 0;
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                //产3干草
                AddCard(new MaterialCard(MaterialType.HAY),3);
                break;
            case MaterialType.HAY:
                //2得3抽收割
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 2 && storageCardType == MaterialType.HAY) {
                    AddExtraDraw((int)SkillType.HARVEST,3);
                    storageCardNum = 0;
                    storageCardType = MaterialType.NULL;
                }
                break;
            case MaterialType.STONE:
                //产2收割
                AddCard(new SkillCard(SkillType.HARVEST),2);
                break;
            case MaterialType.MEAT:
                //产1兔
                SpawnAnimal(AnimalType.RABBIT);
                break;
            default:
                break;
        }
    }
    public override void ExtraEffect() {
        //收割额外+1充 (这个效果在收割技能里实现)
    }
}
