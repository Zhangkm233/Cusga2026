using UnityEngine;

[CreateAssetMenu(fileName = "LandSO",menuName = "Land/LandScriptableObject")]
public class LandScriptableObject : ScriptableObject
{
    [SerializeField] private string landName;
    [SerializeField] private string landDescription;
    [SerializeField] private string landPassiveEffectInfo;
    [SerializeField] private string landExtraEffectInfo;
    [SerializeField] private int landWoodUpgradeNeed;
    [SerializeField] private string landWoodUpgradeInfo;
    [SerializeField] private int landHayUpgradeNeed;
    [SerializeField] private string landHayUpgradeInfo;
    [SerializeField] private int landStoneUpgradeNeed;
    [SerializeField] private string landStoneUpgradeInfo;
    [SerializeField] private int landMeatUpgradeNeed;
    [SerializeField] private string landMeatUpgradeInfo;
    [SerializeField] private LandType landType;
    [SerializeField] private int landLevel;
    [SerializeField] private int landRequiredEnergy;
    [SerializeField] private int landPreference;
    [SerializeField] private GameObject landPrefab;

    public string LandName => landName;
    public string LandDescription => landDescription;
    public int LandLevel => landLevel;
    public int LandRequiredEnergy => landRequiredEnergy;
    public int LandPreference => landPreference;
    public LandType LandType => landType;
    public GameObject LandPrefab => landPrefab;
    public string LandPassiveEffectInfo => landPassiveEffectInfo;
    public string LandExtraEffectInfo => landExtraEffectInfo;
    public string LandHayUpgradeInfo => landHayUpgradeInfo;
    public string LandWoodUpgradeInfo => landWoodUpgradeInfo;
    public string LandStoneUpgradeInfo => landStoneUpgradeInfo;
    public string LandMeatUpgradeInfo => landMeatUpgradeInfo;
    public int LandWoodUpgradeNeed => landWoodUpgradeNeed;
    public int LandHayUpgradeNeed => landHayUpgradeNeed;
    public int LandStoneUpgradeNeed => landStoneUpgradeNeed;
    public int LandMeatUpgradeNeed => landMeatUpgradeNeed;


}
