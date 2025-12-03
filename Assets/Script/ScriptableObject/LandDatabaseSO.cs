using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 地形数据库ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "LandDatabaseSO",menuName = "Land/LandDatabaseSO")]
public class LandDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public class LandEntry
    {
        public int landId;
        public LandType landType;
        public LandScriptableObject landSO;
    }

    [SerializeField]
    private List<LandEntry> landEntries = new List<LandEntry>();
    [SerializeField]
    private Dictionary<int,LandScriptableObject> idDict;
    [SerializeField]
    private Dictionary<LandType,LandScriptableObject> typeDict;
    private bool isInitialized = false;

    public void InitDictionary() {
        if (isInitialized) return;
        idDict = new Dictionary<int,LandScriptableObject>();
        typeDict = new Dictionary<LandType,LandScriptableObject>();
        foreach (var entry in landEntries) {
            if (!idDict.ContainsKey(entry.landId))
                idDict.Add(entry.landId,entry.landSO);
            if (!typeDict.ContainsKey(entry.landType))
                typeDict.Add(entry.landType,entry.landSO);
        }
        isInitialized = true;
    }

    public LandScriptableObject GetLandSOById(int landId) {
        if (!isInitialized) InitDictionary();
        idDict.TryGetValue(landId,out var landSO);
        return landSO;
    }

    public LandScriptableObject GetLandSOByType(LandType landType) {
        if (!isInitialized) InitDictionary();
        typeDict.TryGetValue(landType,out var landSO);
        return landSO;
    }
}
