using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void OnClickTurnEndButton() {
        GameManager.Instance.EndTurn();
    }
}
