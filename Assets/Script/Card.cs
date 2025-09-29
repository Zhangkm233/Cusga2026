using UnityEngine;

public enum CardType
{
    MATERIAL = 0, //材料
    SKILL = 1, //技能
    WEAPON = 2, //武器
    DISASTER = 3, //灾难
}

public enum MaterialType
{
    NULL = 0, //无
    HAY = 0001, //稻草
    WOOD = 0002, //木头
    STONE = 0003, //石头
    MEAT = 0004, //肉
}

public enum SkillType
{
    NULL = 1000, //无
    HARVEST = 1001, //收割
    REINFORCE = 1002, //加固
    STALK = 1003, //追踪
}

public enum WeaponType
{
    NULL = 2000, //无
    ROLLROCK = 2001, //滚石
    SPEAR = 2002, //长矛
    BOW = 2003, //弓
}

public enum DisasterType
{
    NULL = 3000, //无
    STORM = 3001, //风暴
    FLOOD = 3002, //泛滥
    BLAZE = 3003, //焚林
}

[System.Serializable]
public abstract class Card
{
    private CardType cardType; //卡片类型
    public CardType CardType {
        get { return cardType; }
        set { cardType = value; }
    }
    private string cardName; //卡片名称

    public string CardName {
        get { return cardName; }
        set { cardName = value; }
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
                CardName = "稻草";
                Description = "稻草*1";
                Id = 0001;
                break;
            case MaterialType.WOOD:
                CardName = "木头";
                Description = "木头*1";
                Id = 0002;
                break;
            case MaterialType.STONE:
                CardName = "石头";
                Description = "石头*1";
                Id = 0003;
                break;
            case MaterialType.MEAT:
                CardName = "肉";
                Description = "肉*1";
                Id = 0004;
                break;
            default:
                CardName = "材料";
                Description = "未知材料类型.";
                Id = 0000;
                break;
        }
    }
    public override void ApplyEffect(Land targetLand) {
        // 应用材料效果到目标土地
        // 效果在地块里
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
                CardName = "收割";
                Description = "该地块获得2能量";
                Id = 1001;
                break;
            case SkillType.REINFORCE:
                // 加固
                CardName = "加固";
                Description = "该地块获得1坚固";
                Id = 1002;
                break;
            case SkillType.STALK:
                // 追踪
                CardName = "追踪";
                Description = "该地块获得1猎圈";
                Id = 1003;
                break;
            default:
                CardName = "Unknown Skill";
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
    public WeaponType weaponType; //武器类型

    public WeaponCard(WeaponType weaponType) {
        CardType = CardType.WEAPON;
        this.weaponType = weaponType;
        switch (weaponType) {
            case WeaponType.ROLLROCK:
                CardName = "滚石";
                Description = "造成1-2伤害，每座山脉额外+2伤害值";
                Id = 2001;
                break;
            case WeaponType.SPEAR:
                CardName = "长矛";
                Description = "造成3-4伤害";
                Id = 2002;
                break;
            case WeaponType.BOW:
                CardName = "弓";
                Description = "造成3-7伤害";
                Id = 2003;
                break;
            default:
                CardName = "Unknown Equipment";
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

public class DisasterCard : Card
{
    private int TypeOfDisaster = 0;
    public DisasterCard() {
        CardType = CardType.DISASTER;
        TypeOfDisaster = Random.Range(1,4);
        switch (TypeOfDisaster) {
            case 1:
                CardName = "风暴";
                Description = "需要耗3木来抵抗这个灾难，抵抗失败，随机地块变为废墟";
                Id = 3001;
                break;
            case 2:
                CardName = "泛滥";
                Description = "需要耗3草来抵抗这个灾难，抵抗失败，摧毁随机3张牌库中的技能卡";
                Id = 3002;
                break;
            case 3:
                CardName = "焚林";
                Description = "需要耗1石来抵抗这个灾难，抵抗失败，随机地块变为废墟，摧毁随机2张牌库中的技能卡";
                Id = 3003;
                break;
            default:
                CardName = "未知灾难";
                Description = "未知灾难效果.";
                Id = 3000;
                break;
        }
    }
    public override void ApplyEffect(Land targetLand) {
        switch (TypeOfDisaster) {
            case 1:
                if (DeckManager.Instance.TryToRemoveCertainCardByIdMultipleTime((int)MaterialType.WOOD,3)) {
                    Debug.Log("成功抵抗灾难");
                } else {
                    // 抵抗失败，随机地块降级为平原
                    Debug.Log("抵抗灾难失败，随机地块变为废墟");
                    MapManager.Instance.TransformRandomLand(LandType.RUIN);
                }
                break;
            case 2:
                if (DeckManager.Instance.TryToRemoveCertainCardByIdMultipleTime((int)MaterialType.HAY,3)) {
                    Debug.Log("成功抵抗灾难");
                } else {
                    // 抵抗失败，随机破坏技能牌库，失去3张技能牌
                    Debug.Log("抵抗灾难失败，摧毁随机3张牌库中的技能卡");
                    DeckManager.Instance.ShuffleDeck();
                    DeckManager.Instance.TryToRemoveCertainCardByTypeMultipleTime(CardType.SKILL,3);
                }
                break;
            case 3:
                if (DeckManager.Instance.TryToRemoveCertainCardByIdMultipleTime((int)MaterialType.STONE,1)) {
                    Debug.Log("成功抵抗灾难");
                } else {
                    // 抵抗失败，随机地块降级为山丘，下回合再抽1张技能牌
                    Debug.Log("抵抗灾难失败，随机地块变为废墟，摧毁随机2张牌库中的技能卡");
                    MapManager.Instance.TransformRandomLand(LandType.RUIN);
                    DeckManager.Instance.TryToRemoveCertainCardByTypeMultipleTime(CardType.SKILL,2);
                }
                break;
            default:
                break;
        }
    }
}