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
    HAY = 1, //干草
    WOOD = 2, //木材
    STONE = 3, //石头
    MEAT = 4, //肉
}

public enum SkillType
{
    NULL = 0, //空
    STALK = 1, //追猎
    HARVEST = 2, //收割
    REINFORCE = 3, //加固
}

public enum EquipType
{
    NULL = 0, //空
    ROLLROCK = 1, //滚石
    SPEAR = 2, //长矛
    BOW = 3, //短弓
}
public abstract class Card
{
    CardType cardType; //卡片类型
    public CardType CardType {
        get { return cardType; }
        set { cardType = value; }
    }
    public string name; //卡片名称
    public string Name {
        get { return name; }
        set { name = value; }
    }
    public string description; //卡片描述
    public string Description {
        get { return description; }
        set { description = value; }
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
                break;
            case MaterialType.WOOD:
                Name = "木材";
                Description = "木材*1";
                break;
            case MaterialType.STONE:
                Name = "石头";
                Description = "石头*1";
                break;
            case MaterialType.MEAT:
                Name = "肉";
                Description = "肉*1";
                break;
            default:
                Name = "Unknown Material";
                Description = "This material type is not recognized.";
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
                break;
            case SkillType.REINFORCE:
                // 加固
                Name = "加固";
                Description = "该地块获得1坚固";
                break;
            case SkillType.STALK:
                // 追猎
                Name = "追猎";
                Description = "该地块获得1猎圈";
                break;
            default:
                Name = "Unknown Skill";
                Description = "This skill type is not recognized.";
                break;
        }
    }
    public override void ApplyEffect(Land targetLand) {
        switch (skillType) {
            case SkillType.HARVEST:
                targetLand.AddEnergy(2);
                targetLand.PassiveEffect();
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
    public EquipType equipType; //装备类型

    public WeaponCard(EquipType equipType) {
        CardType = CardType.WEAPON;
        this.equipType = equipType;
        switch (equipType) {
            case EquipType.ROLLROCK:
                Name = "滚石";
                Description = "打1-2，每个山脉额外+2最大值";
                break;
            case EquipType.SPEAR:
                Name = "长矛";
                Description = "打3-4";
                break;
            case EquipType.BOW:
                Name = "弓";
                Description = "打3-7";
                break;
            default:
                Name = "Unknown Equipment";
                Description = "This equipment type is not recognized.";
                break;
        }
    }

    public override void ApplyEffect(Land targetLand) {
        //这里要写攻击的逻辑，还没写
        switch (equipType) {
            case EquipType.ROLLROCK:
                break;
            case EquipType.SPEAR:
                break;
            case EquipType.BOW:
                break;
            default:
                break;
        }
    }
}