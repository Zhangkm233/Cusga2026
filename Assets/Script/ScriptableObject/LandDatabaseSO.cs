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
    private Dictionary<int,LandScriptableObject> idDict;
    private Dictionary<LandType,LandScriptableObject> typeDict;
    private bool isInitialized = false;

    public void InitDictionary() {
        if (!isInitialized) return;
        idDict = new Dictionary<int,LandScriptableObject>();
        typeDict = new Dictionary<LandType,LandScriptableObject>();
        foreach (var entry in landEntries) {
            if (entry == null || entry.landSO == null) continue;
            if (idDict.ContainsKey(entry.landId)) {
                Debug.LogError($"LandDatabaseSO: 重复的 landId: {entry.landId}，请检查配置！");
                continue;
            }
            idDict.Add(entry.landId, entry.landSO);

            if (typeDict.ContainsKey(entry.landType)) {
                Debug.LogError($"LandDatabaseSO: 重复的 landType: {entry.landType}，请检查配置！");
                continue;
            }
            typeDict.Add(entry.landType, entry.landSO);
        }
        isInitialized = true;
        Debug.Log($"LandDatabaseSO初始化完成，包含{landEntries.Count}个地形条目");
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
