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

    public void OnClickPauseGameButton() {
        if (GameManager.Instance.stateMachine.CurrentPhase == GamePhase.PlayerTurn) {
            GameManager.Instance.PauseGame();
        } else if (GameManager.Instance.stateMachine.CurrentPhase == GamePhase.GamePaused) {
            GameManager.Instance.ContinueGame();
        } else {
            Debug.Log("当前无法暂停或继续游戏");
        }
    }

    public void OnClickHandShowerButton() {
        DeckManager.Instance.ShowHand();
    }

    public void OnClickDeckShowerButton() {
        DeckManager.Instance.ShowDeck();
    }
}
