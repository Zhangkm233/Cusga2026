using UnityEngine;

/// <summary>
/// 卡牌ScriptableObject，提供卡牌显示的数据（名称、描述、类型、图片等）
/// 注意：不是卡牌的基类
/// </summary>
[CreateAssetMenu(fileName = "CardSO",menuName = "Card/CardScriptableObject")]
public class CardScriptableObject : ScriptableObject
{
    [SerializeField] private string cardName;
    [SerializeField] private string cardDescription;
    [SerializeField] private CardType cardType;
    [SerializeField] private Sprite cardImage;

    public string CardName => cardName;
    public string CardDescription => cardDescription;
    public CardType CardType => cardType;
    public Sprite CardImage => cardImage;
}
