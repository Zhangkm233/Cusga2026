using UnityEngine;

public enum CardType
{
    MATERIAL = 0, //材料
    SKILL = 1, //技能
    EQUIP = 2, //装备
}

public enum MaterialType
{
    NULL = 0, //空
    HAY = 1, //干草
    WOOD = 2, //木材
    STONE = 3, //石头
}

public enum SkillType
{
    NULL = 0, //空
    HARVEST = 1, //收割
    REINFORCE = 2, //加固
    POLISH = 3, //打磨
}

public enum EquipType
{
    NULL = 0, //空
    ROLLROCK = 1, //滚石
    SPEAR = 2, //长矛
    BOW = 3, //弓
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
            default:
                Name = "Unknown Material";
                Description = "This material type is not recognized.";
                break;
        }
    }
    public override void ApplyEffect(Land targetLand) {
        // 应用材料效果到目标地形
        // 效果在地形上
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
                Name = "收割";
                Description = "使一个地块充能+2";
                break;
            case SkillType.REINFORCE:
                Name = "加固";
                Description = "加固一个地块，使其可以抵抗一次攻击";
                break;
            case SkillType.POLISH:
                Name = "打磨";
                Description = "使一个地块ATK+2";
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
                targetLand.energyCounter += 2;
                targetLand.PassiveEffect();
                break;
            case SkillType.REINFORCE:
                targetLand.def += 1;
                break;
            case SkillType.POLISH:
                targetLand.atk += 2;
                break;
        }
    }
}

public class EquipCard : Card
{
    public int attackBonus; //攻击加成
    public EquipType equipType; //装备类型

    public EquipCard(EquipType equipType) {
        CardType = CardType.EQUIP;
        this.equipType = equipType;
        switch (equipType) {
            case EquipType.ROLLROCK:
                Name = "滚石";
                Description = "+1ATK,每控制1个山脉额外+2";
                break;
            case EquipType.SPEAR:
                Name = "长矛";
                Description = "+5ATK";
                break;
            case EquipType.BOW:
                Name = "弓";
                Description = "+7ATK";
                break;
            default:
                Name = "Unknown Equipment";
                Description = "This equipment type is not recognized.";
                break;
        }
    }

    public override void ApplyEffect(Land targetLand) {

    }
}