using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    //用于存储游戏中的全局数据 提供一些静态方法
    public static int turnCount = 0;
    public static string HanizeLandType(LandType landType) {
        //这里需要更新
        switch (landType) {
            case LandType.HILL:
                return "山地";
            case LandType.PLAIN:
                return "平原";
            case LandType.FOREST:
                return "森林";
            case LandType.TOWN:
                return "小镇";
            case LandType.MOUNTAIN:
                return "山脉";
            case LandType.JUNGLE:
                return "密林";
            case LandType.THATCH:
                return "茅屋";
            case LandType.CABIN:
                return "林屋";
            case LandType.WAREHOUSE:
                return "仓库";
            case LandType.TOWER:
                return "塔";
            case LandType.WHEATLAND:
                return "麦田";
            case LandType.WINDMILL:
                return "风车";
            default:
                return "未知";
        }
    }

    public static string HanizeCardType(CardType cardType) {
        switch (cardType) {
            case CardType.MATERIAL:
                return "材料卡";
            case CardType.SKILL:
                return "技能卡";
            case CardType.WEAPON:
                return "武器卡";
            default:
                return "未知";
        }
    }

    public static string HanizeMaterial(MaterialType materialType) { switch (materialType) {
            case MaterialType.NULL:
                return " ";
            case MaterialType.HAY:
                return "干草";
            case MaterialType.WOOD:
                return "木材";
            case MaterialType.STONE:
                return "石头";
            case MaterialType.MEAT:
                return "肉";
            default:
                return "未知";
        }
    }

    public static string HanizeSkill(SkillType skillType) {
        switch (skillType) {
            case SkillType.NULL:
                return " ";
            case SkillType.STALK:
                return "追猎";
            case SkillType.HARVEST:
                return "收割";
            case SkillType.REINFORCE:
                return "加固";
            default:
                return "未知";
        }
    }
}
