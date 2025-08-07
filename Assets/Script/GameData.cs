using UnityEngine;

public static class GameData
{
    public static int extraDrawNum = 0; // 额外抓牌数
    public static string HanizeLandType(LandType landType) {
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
            case LandType.FORTRESS:
                return "要塞";
            case LandType.THATCH:
                return "茅屋";
            case LandType.CABIN:
                return "林屋";
            case LandType.WAREHOUSE:
                return "仓库";
            case LandType.TOWER:
                return "塔";
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
            case CardType.EQUIP:
                return "装备卡";
            default:
                return "未知";
        }
    }
}
