using UnityEngine;


public class PlayerTurnState : IState {
    private GameController gameManager;

    public PlayerTurnState(GameController manager) {
        gameManager = manager;
    }

    public void EnterState() {
        Debug.Log("Player's Turn: Match Blocks!");
        gameManager.Matchable();
    }

    public void UpdateState() {
        // Wait for player to match blocks
        if (gameManager.isEnemyDie()) {
            gameManager.ChangeState(new DeathState(gameManager));
        }
        else {
            if (gameManager.HasPlayerMoved) {
                gameManager.ChangeState(new EnemyTurnState(gameManager));
            }
        }
    }

    public void ExitState() {
        Debug.Log("Player Turn Ended.");
        gameManager.UnMatchable();
    }
}
