using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void OnClickTurnEndButton() {
        if (GameManager.Instance.stateMachine.CurrentPhase == GamePhase.PlayerTurn) {
            GameManager.Instance.EndTurn();
        } else {
            Debug.Log("当前不在玩家回合，无法结束回合");
        }
    }
}
