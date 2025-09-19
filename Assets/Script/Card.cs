using UnityEngine;

public enum CardType
{
    MATERIAL = 0, //材料
    SKILL = 1, //技能
    WEAPON = 2, //武器
}

public enum MaterialType
{
    NULL = 0, //空
    HAY = 0001, //干草
    WOOD = 0002, //木材
    STONE = 0003, //石头
    MEAT = 0004, //肉
}

public enum SkillType
{
    NULL = 1000, //空
    HARVEST = 1001, //收割
    REINFORCE = 1002, //加固
    STALK = 1003, //追猎
}

public enum WeaponType
{
    NULL = 2000, //空
    ROLLROCK = 2001, //滚石
    SPEAR = 2002, //长矛
    BOW = 2003, //短弓
}
public abstract class Card
{
    private CardType cardType; //卡片类型
    public CardType CardType {
        get { return cardType; }
        set { cardType = value; }
    }
    private string name; //卡片名称

    public string Name {
        get { return name; }
        set { name = value; }
    }
    private string description; //卡片描述
    public string Description {
        get { return description; }
        set { description = value; }
    }

    private int id; //卡片id
    public int Id {
        get { return id; }
        set { id = value; }
    }

    public abstract void ApplyEffect(Land targetLand);
}

public class MaterialCard : Card
{
    public MaterialType materialType; //材料类型
    public MaterialType MaterialType {
        get { return materialType; }
        set { materialType = value; }
    }
    public MaterialCard(MaterialType type) {
        CardType = CardType.MATERIAL;
        MaterialType = type;
        switch (type) {
            case MaterialType.HAY:
                Name = "干草";
                Description = "干草*1";
                Id = 0001;
                break;
            case MaterialType.WOOD:
                Name = "木材";
                Description = "木材*1";
                Id = 0002;
                break;
            case MaterialType.STONE:
                Name = "石头";
                Description = "石头*1";
                Id = 0003;
                break;
            case MaterialType.MEAT:
                Name = "肉";
                Description = "肉*1";
                Id = 0004;
                break;
            default:
                Name = "——";
                Description = "未知材料类型.";
                Id = 0000;
                break;
        }
    }
    public override void ApplyEffect(Land targetLand) {
        // 应用材料效果到目标地形
        // 效果在地形上
        targetLand.MaterialEffect(this);
    }
}

public class SkillCard : Card
{
    public SkillType skillType; //技能类型
    public SkillCard(SkillType skillType) {
        CardType = CardType.SKILL;
        this.skillType = skillType;
        switch (skillType) {
            case SkillType.HARVEST:
                // 收割
                Name = "收割";
                Description = "该地块获得2充能";
                Id = 1001;
                break;
            case SkillType.REINFORCE:
                // 加固
                Name = "加固";
                Description = "该地块获得1坚固";
                Id = 1002;
                break;
            case SkillType.STALK:
                // 追猎
                Name = "追猎";
                Description = "该地块获得1猎圈";
                Id = 1003;
                break;
            default:
                Name = "Unknown Skill";
                Description = "This skill type is not recognized.";
                Id = 0000;
                break;
        }
    }
    public override void ApplyEffect(Land targetLand) {
        switch (skillType) {
            case SkillType.HARVEST:
                targetLand.AddEnergy(2 + MapManager.Instance.GetAmountOfLandType(LandType.WINDMILL));
                break;
            case SkillType.REINFORCE:
                targetLand.AddSoild(1);
                break;
            case SkillType.STALK:
                targetLand.AddHunterarea(1);
                break;
        }
    }
}

public class WeaponCard : Card
{
    public WeaponType weaponType; //装备类型

    public WeaponCard(WeaponType weaponType) {
        CardType = CardType.WEAPON;
        this.weaponType = weaponType;
        switch (weaponType) {
            case WeaponType.ROLLROCK:
                Name = "滚石";
                Description = "打1-2，每个山脉额外+2最大值";
                Id = 2001;
                break;
            case WeaponType.SPEAR:
                Name = "长矛";
                Description = "打3-4";
                Id = 2002;
                break;
            case WeaponType.BOW:
                Name = "弓";
                Description = "打3-7";
                Id = 2003;
                break;
            default:
                Name = "Unknown Equipment";
                Description = "This equipment type is not recognized.";
                Id = 0000;
                break;
        }
    }

    public override void ApplyEffect(Land targetLand) {
        switch (weaponType) {
            case WeaponType.ROLLROCK:
                MapManager.Instance.DealDamageTo(targetLand,Random.Range(1,3) + (MapManager.Instance.GetAmountOfLandType(LandType.MOUNTAIN) * 2) 
                    + MapManager.Instance.GetAmountOfLandType(LandType.TOWER));
                break;
            case WeaponType.SPEAR:
                MapManager.Instance.DealDamageTo(targetLand,Random.Range(3,5)
                    + MapManager.Instance.GetAmountOfLandType(LandType.TOWER));
                break;
            case WeaponType.BOW:
                MapManager.Instance.DealDamageTo(targetLand,Random.Range(3,8)
                    + MapManager.Instance.GetAmountOfLandType(LandType.TOWER));
                break;
            default:
                break;
        }
    }
}