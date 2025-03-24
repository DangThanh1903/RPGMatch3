using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : IState
{
    private GameController gameManager;

    public DeathState(GameController manager) {
        gameManager = manager;
    }

    public void EnterState() {
        gameManager.CombatEnd();
    }

    public void UpdateState() {

    }

    public void ExitState() {

    }
}
