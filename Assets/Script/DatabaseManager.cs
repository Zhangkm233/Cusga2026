using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }
    public CardDatabaseSO cardDatabaseSO;
    public LandDatabaseSO landDatabaseSO;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public CardScriptableObject GetCardSO(int cardId) {
        if (cardDatabaseSO == null) {
            Debug.LogWarning("CardDatabaseSO未分配");
            return null;
        }
        return cardDatabaseSO.GetCardSO(cardId);
    }

    public LandScriptableObject GetLandSOById(int landId) {
        if (landDatabaseSO == null) {
            Debug.LogWarning("LandDatabaseSO未分配");
            return null;
        }
        return landDatabaseSO.GetLandSOById(landId);
    }

    public LandScriptableObject GetLandSOByType(LandType landType) {
        if (landDatabaseSO == null) {
            Debug.LogWarning("LandDatabaseSO未分配");
            return null;
        }
        return landDatabaseSO.GetLandSOByType(landType);
    }
}