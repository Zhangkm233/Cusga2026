using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public float rotationSpeed = 30f;
    public float distanceFromTarget = 10f;
    public float zoomSpeed = 0.1f;
    private float minDistance = 8f;
    private float maxDistance = 25f;


    [SerializeField]
    private bool autoCalibrateSight = false;

    public GameObject target; //������

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
        if (autoCalibrateSight) {
            ChangeSightByDistance();
        }
    }

    public void RotateSight(int speed) {
        target.transform.Rotate(0,speed * rotationSpeed * Time.deltaTime,0);
    }

    public void SetTarget(GameObject newTarget) {
        target = newTarget;
    }

    public void HorizonMoveSightByKey() {
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed) {
            RotateSight(1);
        } else if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed) {
            RotateSight(-1);
        }
    }

    public void ChangeDistanceByKey() {
        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed) {
            distanceFromTarget -= zoomSpeed;
            if (distanceFromTarget < minDistance) {
                distanceFromTarget = minDistance;
            }
        } else if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed) {
            distanceFromTarget += zoomSpeed;
            if (distanceFromTarget > maxDistance) {
                distanceFromTarget = maxDistance;
            }
        }
    }

    public void ChangeSightByDistance() {
        // ���������Ŀ��ľ���
        if (target != null) {
            Vector3 direction = (transform.position - target.transform.position).normalized;
            transform.position = target.transform.position + direction * distanceFromTarget;
            transform.LookAt(target.transform);
        }
    }
}
