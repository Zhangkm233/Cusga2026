using System.Collections;
using UnityEngine;

// 协程处理器
public class CoroutineHandler : MonoBehaviour
{
    private static CoroutineHandler _instance;

    public static CoroutineHandler Instance {
        get {
            if (_instance == null) {
                GameObject obj = new GameObject("CoroutineHandler");
                _instance = obj.AddComponent<CoroutineHandler>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }
}