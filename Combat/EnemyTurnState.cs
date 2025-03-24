using UnityEngine;

public class EnemyTurnState : IState {
    private GameController gameManager;

    public EnemyTurnState(GameController manager) {
        gameManager = manager;
    }

    public void EnterState() {
        Debug.Log("Enemy's Turn: Attacking...");
        gameManager.EnemyAttack();
    }

    public void UpdateState() {
        if (gameManager.isPlayerDie()) {
            gameManager.ChangeState(new DeathState(gameManager));
        }
        else {
            gameManager.ChangeState(new PlayerTurnState(gameManager));
        }
    }

    public void ExitState() {
        Debug.Log("Enemy Turn Ended.");
    }
}
