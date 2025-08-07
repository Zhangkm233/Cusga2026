using UnityEngine;

public enum LandType
{
    HILL = 0, //山丘
    PLAIN = 1, //平原
    FOREST = 2, //森林
    THATCH = 3, //茅屋
    CABIN = 4, //林屋
    MOUNTAIN = 5, //山脉
    TOWN = 6, //城镇
    TOWER = 7, //塔
    FORTRESS = 8, //堡垒
    WAREHOUSE = 9, //仓库
    DESERT = 10, //沙漠
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
    public MaterialType storageCardType = MaterialType.NULL; //存储的卡片类别
    public int storageCardNum = 0; //存储的卡片数量
    public int atk = 0;
    public int def = 0;

    public int EnergyCounter {
        get { return energyCounter; }
        set { energyCounter = value; }
    }

    public void AddEnergy(int energy) {
        energyCounter += energy; // 增加充能
        Debug.Log($"地形 {landType} 充能增加 {energy}，当前充能：{energyCounter}");
    }
    public void AddDef(int def) {
        this.def += def; // 增加防御力
        Debug.Log($"地形 {landType} 防御力增加 {def}，当前防御力：{this.def}");
    }

    public abstract void PassiveEffect(); //被动效果
    public abstract void MaterialEffect(MaterialCard materialCard);

    public void StorageIn(MaterialType materialType) {
        if (storageCardType == materialType) {
            storageCardNum += 1; // 存储的卡片数量增加1
        } else {
            storageCardType = materialType; // 存储的卡片类型为材料
            storageCardNum = 1; // 存储的卡片数量设置为1
        }
    }

    public void ChangeLandType(LandType landType) {
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
            case LandType.FORTRESS:
                newLand = this.gameObject.AddComponent<FortressLand>();
                break;
            case LandType.WAREHOUSE:
                newLand = this.gameObject.AddComponent<WarehouseLand>();
                break;
            case LandType.DESERT:
                newLand = this.gameObject.AddComponent<DesertLand>();
                break;
        }
        if (landType != LandType.DESERT) {
            newLand.atk = this.atk; // 继承攻击力
            newLand.def = this.def; // 继承防御力
        }
        Destroy(this); // 销毁当前地形组件
    }
}

public class HillLand : Land 
{
    public HillLand() {
        LandType = LandType.HILL;
        requiredEnergy = 4;
    }
    public HillLand(int atk,int def) {
        LandType = LandType.HILL;
        requiredEnergy = 4;
        this.atk = atk;
        this.def = def;
    }

    public override void PassiveEffect() {
        // 山丘地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("山丘地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.STONE)); //添加一个石头材料卡到牌库
            energyCounter = 0; // 重置计数器
        } else {
        }
    }

    public override void MaterialEffect(MaterialCard materialCard) {
        switch(materialCard.materialType) {
            case MaterialType.HAY:
                //产5木材
                DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.WOOD),5);
                break;
            case MaterialType.WOOD:
                //产1干草
                DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.HAY), 1);
                break;
            case MaterialType.STONE:
                //变山脉
                ChangeLandType(LandType.MOUNTAIN);
                break;
            default:
                break;
        }
    }
}

public class PlainLand : Land
{
    public PlainLand() {
        LandType = LandType.PLAIN;
        requiredEnergy = 2;
    }
    public PlainLand(int atk,int def) {
        LandType = LandType.PLAIN;
        requiredEnergy = 2;
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 平原地形的被动效果 2时产草
        if (energyCounter >= requiredEnergy) {
            Debug.Log("平原地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.HAY)); //添加一个干草材料卡到牌库
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.HAY:
                //3个以变茅屋
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 3 ) {
                    ChangeLandType(LandType.THATCH);
                }
                break;
            case MaterialType.WOOD:
                //变森林产干草
                DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.HAY),1);
                ChangeLandType(LandType.FOREST);
                break;
            case MaterialType.STONE:
                //变山丘产2干草
                DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.HAY),2);
                ChangeLandType(LandType.HILL);
                break;
            default:
                break;
        }
    }
}

public class ForestLand : Land
{
    public ForestLand() {
        LandType = LandType.FOREST;
        requiredEnergy = 1;
    }
    public ForestLand(int atk,int def) {
        LandType = LandType.FOREST;
        requiredEnergy = 1;
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 森林地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("森林地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.WOOD)); //添加一个木材材料卡到牌库
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.HAY:
                //变平原产2木头
                DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.WOOD),2);
                ChangeLandType(LandType.PLAIN);
                break;
            case MaterialType.WOOD:
                //4x变林屋
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 4) {
                    ChangeLandType(LandType.CABIN);
                }
                break;
            case MaterialType.STONE:
                //变山丘产3木头
                DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.WOOD),3);
                ChangeLandType(LandType.HILL);
                break;
            default:
                break;
        }
    }
}

public class ThatchLand : Land
{
    public ThatchLand() {
        LandType = LandType.THATCH;
        requiredEnergy = 4;
    }
    public ThatchLand(int atk,int def) {
        LandType = LandType.THATCH;
        requiredEnergy = 4;
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 茅屋地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("茅屋地形被动效果触发");
            //4时产1抽 
            GameData.extraDrawNum++;
            energyCounter = 0; // 重置计数器
        } else {
        }
    }

    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.HAY:
                //产“加固”
                DeckManager.Instance.AddCardToDeck(new SkillCard(SkillType.REINFORCE));
                break;
            case MaterialType.WOOD:
                //2x产矛
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 2) {
                    DeckManager.Instance.AddCardToDeck(new EquipCard(EquipType.SPEAR));
                    storageCardNum = 0; // 重置存储的卡片数量
                }
                this.gameObject.AddComponent<ForestLand>();
                Destroy(this); // 销毁当前地形组件
                break;
            case MaterialType.STONE:
                //2x变小镇
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 2) {
                    ChangeLandType(LandType.TOWN);
                }
                break;
            default:
                break;
        }
    }
}

public class CabinLand : Land
{
    public CabinLand() {
        LandType = LandType.CABIN;
        requiredEnergy = 3;
    }
    public CabinLand(int atk,int def) {
        LandType = LandType.CABIN;
        requiredEnergy = 3;
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 林屋地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("林屋地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new SkillCard(SkillType.HARVEST)); 
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.HAY:
                //3x变小镇
                StorageIn(MaterialType.HAY);
                if (storageCardNum >= 3) {
                    ChangeLandType(LandType.TOWN);

                }
                break;
            case MaterialType.WOOD:
                //2x产收割
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 2) {
                    DeckManager.Instance.AddCardToDeck(new SkillCard(SkillType.HARVEST));
                    storageCardNum = 0; // 重置存储的卡片数量
                }
                break;
            case MaterialType.STONE:
                //产2滚石
                DeckManager.Instance.AddCardToDeck(new EquipCard(EquipType.ROLLROCK),2);
                break;
            default:
                break;
        }
    }
}

public class MountainLand : Land
{
    public MountainLand() {
        LandType = LandType.MOUNTAIN;
        requiredEnergy = 4;
    }
    public MountainLand(int atk,int def) {
        LandType = LandType.MOUNTAIN;
        requiredEnergy = 4;
        this.atk = atk;
        this.def = def;
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
            case MaterialType.HAY:
                //加1充
                AddEnergy(1);
                break;
            case MaterialType.WOOD:
                //2x加盾
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 2) {
                    AddDef(1);
                    storageCardNum = 0; // 重置存储的卡片数量
                }
                break;
            case MaterialType.STONE:
                //产2滚石
                DeckManager.Instance.AddCardToDeck(new EquipCard(EquipType.ROLLROCK),2);
                break;
            default:
                break;
        }
    }
}

public class TownLand : Land
{
    public TownLand() {
        LandType = LandType.TOWN;
        requiredEnergy = 5;
    }
    public TownLand(int atk,int def) {
        LandType = LandType.TOWN;
        requiredEnergy = 5;
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 城镇地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("城镇地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.HAY)); //添加一个干草材料卡到牌库
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.WOOD)); //添加一个木材材料卡到牌库
            DeckManager.Instance.AddCardToDeck(new MaterialCard(MaterialType.STONE)); //添加一个石头材料卡到牌库
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.HAY:
                //产矛
                DeckManager.Instance.AddCardToDeck(new EquipCard(EquipType.SPEAR));
                break;
            case MaterialType.WOOD:
                //3x变仓库
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 3) {
                    ChangeLandType(LandType.WAREHOUSE);
                }
                break;
            case MaterialType.STONE:
                //3x变塔
                StorageIn(MaterialType.STONE);
                if (storageCardNum >= 3) {
                    ChangeLandType(LandType.TOWER);
                }
                break;
            default:
                break;
        }
    }
}

public class TowerLand : Land
{
    public TowerLand() {
        LandType = LandType.TOWER;
        requiredEnergy = 10;
    }
    public TowerLand(int atk,int def) {
        LandType = LandType.TOWER;
        requiredEnergy = 10;
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 塔地形的被动效果
        // 将收割的效果改为使一个地块充能+3
    }

    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.HAY:
                //产加固
                DeckManager.Instance.AddCardToDeck(new SkillCard(SkillType.REINFORCE));
                break;
            case MaterialType.WOOD:
                //3x变壁垒
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 3) {
                    ChangeLandType(LandType.FORTRESS);
                }
                break;
            case MaterialType.STONE:
                //变璧垒
                ChangeLandType(LandType.FORTRESS);
                break;
            default:
                break;
        }
    }
}

public class FortressLand : Land
{
    public FortressLand() {
        LandType = LandType.FORTRESS;
        requiredEnergy = 3;
    }
    public FortressLand(int atk,int def) {
        LandType = LandType.FORTRESS;
        requiredEnergy = 3;
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 堡垒地形的被动效果 3时产长弓
        if (energyCounter >= requiredEnergy) {
            Debug.Log("堡垒地形被动效果触发");
            DeckManager.Instance.AddCardToDeck(new EquipCard(EquipType.BOW)); //添加一个长弓装备卡到牌库
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.HAY:
                //产收割
                DeckManager.Instance.AddCardToDeck(new SkillCard(SkillType.HARVEST));
                break;
            case MaterialType.WOOD:
                //2x产长弓
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 2) {
                    DeckManager.Instance.AddCardToDeck(new EquipCard(EquipType.BOW));
                    storageCardNum = 0; // 重置存储的卡片数量
                }
                break;
            case MaterialType.STONE:
                //产1抽
                GameData.extraDrawNum++;
                break;
            default:
                break;
        }
    }
}

public class WarehouseLand : Land
{
    public WarehouseLand() {
        LandType = LandType.WAREHOUSE;
        requiredEnergy = 2;
    }
    public WarehouseLand(int atk,int def) {
        LandType = LandType.WAREHOUSE;
        requiredEnergy = 2;
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 仓库地形的被动效果
        if (energyCounter >= requiredEnergy) {
            Debug.Log("仓库地形被动效果触发");
            // 2时产1抽
            GameData.extraDrawNum++; // 抽一张牌
            energyCounter = 0; // 重置计数器
        } else {
        }
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        switch (materialCard.materialType) {
            case MaterialType.HAY:
                //加1充
                AddEnergy(1);
                break;
            case MaterialType.WOOD:
                //3x加盾
                StorageIn(MaterialType.WOOD);
                if (storageCardNum >= 3) {
                    AddDef(1);
                    storageCardNum = 0; // 重置存储的卡片数量
                }
                break;
            case MaterialType.STONE:
                //产1打磨
                DeckManager.Instance.AddCardToDeck(new SkillCard(SkillType.POLISH));
                break;
            default:
                break;
        }
    }
}

public class DesertLand : Land
{
    public DesertLand() {
        LandType = LandType.DESERT;
        requiredEnergy = 0; // 沙漠地形没有被动效果
    }
    public DesertLand(int atk,int def) {
        LandType = LandType.DESERT;
        requiredEnergy = 0; // 沙漠地形没有被动效果
        this.atk = atk;
        this.def = def;
    }
    public override void PassiveEffect() {
        // 沙漠地形没有被动效果
    }
    public override void MaterialEffect(MaterialCard materialCard) {
        // 沙漠地形没有材料效果
    }
}