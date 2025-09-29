using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public float rotationSpeed = 30f;
    public float distanceFromTarget = 10f;
    public float zoomSpeed = 0.3f;

    public GameObject target; //父物体

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (target == null) {
            target = this.transform.parent.gameObject;
        }
    }

    void Update() {
        HorizonMoveSightByKey();
        ChangeDistanceByKey();
        ChangeSightByDistance();
    }

    public void RotateSight(int speed) {
        target.transform.Rotate(0,speed * rotationSpeed * Time.deltaTime,0);
    }

    public void SetTarget(GameObject newTarget) {
        target = newTarget;
    }

    public void HorizonMoveSightByKey() {
        if (Keyboard.current.rightArrowKey.isPressed) {
            RotateSight(-1);
        } else if (Keyboard.current.leftArrowKey.isPressed) {
            
            RotateSight(1);
        }
    }

    public void ChangeDistanceByKey() {
        if (Keyboard.current.upArrowKey.isPressed) {
            distanceFromTarget -= zoomSpeed; // 调整缩放速度
            distanceFromTarget = Mathf.Clamp(distanceFromTarget,5f,20f); // 限制缩放范围
        } else if (Keyboard.current.downArrowKey.isPressed) {
            distanceFromTarget += zoomSpeed; // 调整缩放速度
            distanceFromTarget = Mathf.Clamp(distanceFromTarget,5f,20f); // 限制缩放范围
        }
    }

    public void ChangeSightByDistance() {
        // 保持相机与目标的距离
        if (target != null) {
            Vector3 direction = (transform.position - target.transform.position).normalized;
            transform.position = target.transform.position + direction * distanceFromTarget;
            transform.LookAt(target.transform);
        }
    }
}
