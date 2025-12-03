using UnityEngine;

[CreateAssetMenu(fileName = "LandSO", menuName = "Land/LandScriptableObject")]
public class LandScriptableObject : ScriptableObject
{
    [SerializeField] private string landName;
    [SerializeField] private string landDescription;
    [SerializeField] private string landPassiveEffectInfo;
    [SerializeField] private string landHayUpgradeInfo;
    [SerializeField] private string landWoodUpgradeInfo;
    [SerializeField] private string landStoneUpgradeInfo;
    [SerializeField] private string landMeatUpgradeInfo;
    [SerializeField] private LandType landType;
    [SerializeField] private int landLevel;
    [SerializeField] private int landRequiredEnergy;
    [SerializeField] private int landPreference;

    public string LandName => landName;
    public string LandDescription => landDescription;
    public int LandLevel => landLevel;
    public int LandRequiredEnergy => landRequiredEnergy;
    public int LandPreference => landPreference;
    public LandType LandType => landType;

}
