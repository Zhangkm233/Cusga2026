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

public abstract class Land : MonoBehaviour
{
    LandType landType; //地形类型
    public LandType LandType {
        get { return landType; }
        set { landType = value; }
    }
    public int energyCounter = 0; //计数器，表示该地形上有多少个充能
    public int requiredEnergy; //触发被动效果需要的充能数量
    public MaterialType storageCardType = MaterialType.NULL; //已存储的卡片类别
    public int storageCardNum = 0; //存储的卡片数量
    public int preference = 0; //偏好地形数值
    public int soild = 0; //坚固值 可以抵挡一次攻击或者天灾效果
    public int hunterarea = 0;//猎圈值 额外收益的概率增加一倍
    public static int maxSoild = 1; //最大坚固值
    public static int maxHunterarea = 2; //最大猎圈值

    public int EnergyCounter {
        get { return energyCounter; }
        set { energyCounter = value; }
    }
    public int Preference {
        get { return preference; }
        set { preference = value; }
    }

    public void AddEnergy(int energy) {
        energyCounter += energy; // 增加充能
        Debug.Log($"地形 {landType} 充能增加 {energy}，当前充能：{energyCounter}");
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
    public void ChangeLandType(LandType landType) {
        //这里是转变地形的函数 如果想解耦合，还需要改一改到游戏里面的调用，应该UI层面需要全部重写
        //需要在逻辑层里面添加存储 然后再显示到游戏/UE层
        Land newLand = null;
        switch (landType) {
            case LandType.HILL:
                newLand =  this.gameObject.AddComponent<HillLand>();
                break;
            case LandType.PLAIN:
                newLand = this.gameObject.AddComponent<PlainLand>();
                break;
            case LandType.FOREST:
                newLand = this.gameObject.AddComponent<ForestLand>();
                break;
            case LandType.THATCH:
                newLand = this.gameObject.AddComponent<ThatchLand>();
                break;
            case LandType.CABIN:
                newLand = this.gameObject.AddComponent<CabinLand>();
                break;
            case LandType.MOUNTAIN:
                newLand = this.gameObject.AddComponent<MountainLand>();
                break;
            case LandType.TOWN:
                newLand = this.gameObject.AddComponent<TownLand>();
                break;
            case LandType.TOWER:
                newLand = this.gameObject.AddComponent<TowerLand>();
                break;
            case LandType.WAREHOUSE:
                newLand = this.gameObject.AddComponent<WarehouseLand>();
                break;
            case LandType.WINDMILL:
                // newLand = this.gameObject.AddComponent<WindmillLand>();
                break;
            case LandType.JUNGLE:
                // newLand = this.gameObject.AddComponent<JungleLand>();
                break;
            case LandType.WHEATLAND:
                // newLand = this.gameObject.AddComponent<WheatlandLand>();
                break;
        }
        //if (landType != LandType.DESERT) {
            //继承所有特殊属性（坚固和猎圈之类的）
            newLand.soild = this.soild;
            newLand.hunterarea = this.hunterarea;
        //}
        Destroy(this); // 销毁当前地形组件
    }
}

public class HillLand : Land 
{
    // 山丘地形
    public HillLand() {
        LandType = LandType.HILL;
        requiredEnergy = 4;
        preference = 20;
    }

    public override void PassiveEffect() {
        // 山丘地形的被动效果 4时产1石
        if (energyCounter >= requiredEnergy) {
            Debug.Log("山丘地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.STONE)); //添加一个石头材料卡到牌库
            energyCounter = 0; // 重置计数器
        } else {
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
                DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.WOOD),2);
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
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
        //这里还没写 应该是20%产1石
    }
}

public class PlainLand : Land
{
    // 平原地形
    public PlainLand() {
        LandType = LandType.PLAIN;
        requiredEnergy = 3;
        preference = 30;
    }
    public override void PassiveEffect() {
        // 平原地形的被动效果 3时产1草
        if (energyCounter >= requiredEnergy) {
            Debug.Log("平原地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.HAY)); //添加一个干草材料卡到牌库
            energyCounter = 0; // 重置计数器
        } else {
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
                //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class ForestLand : Land
{
    // 森林地形
    public ForestLand() {
        LandType = LandType.FOREST;
        requiredEnergy = 2;
        preference = 35;
    }

    public override void PassiveEffect() {
        // 森林地形的被动效果 2时产1木
        if (energyCounter >= requiredEnergy) {
            Debug.Log("森林地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.WOOD)); //添加一个木材材料卡到牌库
            energyCounter = 0; // 重置计数器
        } else {
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
                break;
            case MaterialType.STONE:
                //产1矛
                break;
            case MaterialType.MEAT:
                //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class MountainLand : Land
{
    public MountainLand() {
        LandType = LandType.MOUNTAIN;
        requiredEnergy = 4;
    }
    public override void PassiveEffect() {
        // 山脉地形的被动效果 4时产2石
        if (energyCounter >= requiredEnergy) {
            Debug.Log("山脉地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.STONE),2);
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}


public class JungleLand : Land
{
    public JungleLand() {
        LandType = LandType.JUNGLE;
        requiredEnergy = 0;
    }
    public override void PassiveEffect() {
        if (energyCounter >= requiredEnergy) {
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class WheatLand : Land
{
    public WheatLand() {
        LandType = LandType.WHEATLAND;
        requiredEnergy = 0;
    }
    public override void PassiveEffect() {
        if (energyCounter >= requiredEnergy) {
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class ThatchLand : Land
{
    public ThatchLand() {
        LandType = LandType.THATCH;
        requiredEnergy = 4;
    }
    public override void PassiveEffect() {
        // 茅屋地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("茅屋地形被动效果触发");
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class CabinLand : Land
{
    public CabinLand() {
        LandType = LandType.CABIN;
        requiredEnergy = 3;
    }
    
    public override void PassiveEffect() {
        // 林屋地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("林屋地形被动效果触发");
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}


public class TownLand : Land
{
    public TownLand() {
        LandType = LandType.TOWN;
        requiredEnergy = 5;
    }
    public override void PassiveEffect() {
        // 城镇地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("城镇地形被动效果触发");
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class TowerLand : Land
{
    public TowerLand() {
        LandType = LandType.TOWER;
        requiredEnergy = 10;
    }
    public override void PassiveEffect() {
        // 塔地形的被动效果
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}


public class WarehouseLand : Land
{
    public WarehouseLand() {
        LandType = LandType.WAREHOUSE;
        requiredEnergy = 2;
    }
    public override void PassiveEffect() {
        // 仓库地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("仓库地形被动效果触发");
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }

    public override void ExtraEffect() {
    }
}

public class WindmillLand : Land
{
    public WindmillLand() {
        LandType = LandType.WINDMILL;
        requiredEnergy = 0;

    }
    public override void PassiveEffect() {
        if (energyCounter >= requiredEnergy) {
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.WOOD:
                break;
            case MaterialType.HAY:
                break;
            case MaterialType.STONE:
                break;
            case MaterialType.MEAT:
            //还没写
            default:
                break;
        }
    }
    public override void ExtraEffect() {
    }
}
