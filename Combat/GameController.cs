using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Match3;

public class GameController : MonoBehaviour {
    public static GameController Instance;
    private IState currentState;
    public MatchManager matchManager;
    public SwapCounter swapCounter;
    public CombatLogManager combatLogManager;
    public EnemyUI _enemyUI;
    public Enemy currentEnemy;
    private Stage _currentStage;

    public bool HasPlayerMoved { get; set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy the new duplicate
            Destroy(gameObject);
        }
    }
    
    private void Update() {
        currentState?.UpdateState();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isInCombat())
        {
            StartCoroutine(FindComponentsWithDelay());
            setPlayerLocation();
        }
        else
        {
            setPlayerLocation(false);
        }
    }
    private bool isInCombat() {
        return matchManager != null && swapCounter != null && combatLogManager != null;
    }
    private void setPlayerLocation(bool isCombat = true) {
        Vector3 playerPos;
        if (isCombat) {
            playerPos = new Vector3(-4, 0, 0);
        }
        else {
            playerPos = new Vector3(-10, 0, 0);
        }
        Player.Instance.transform.position = playerPos;
    }

    private IEnumerator FindComponentsWithDelay()
    {
        yield return new WaitForSeconds(0.1f); 
        ChangeState(new PlayerTurnState(this)); 
    }

    public void ChangeState(IState newState) {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    public void SetCurrentEnemie(Enemy e) {
        currentEnemy = e;
    }

    public void PlayerMoved() {
        HasPlayerMoved = true;
    }

    public void EnemyAttack() {
        StartCoroutine(EnemyAttackAnimation());
        StartCoroutine(PlayerTakingDamage());
        HasPlayerMoved = false;
    }
    private IEnumerator PlayerTakingDamage() {
        Debug.Log("Enemy deals damage!");
        Player.Instance.TakeDamage(_enemyUI.damage);
        if (combatLogManager != null)
        {
            combatLogManager.AddLog($"Enemy dealt {_enemyUI.damage} damage!");
        }
        yield return new WaitForSeconds(1.2f);
    }
    private IEnumerator EnemyAttackAnimation() {
        _enemyUI.AttackAnima();
        yield return new WaitForSeconds(_enemyUI.attackMoveDuration);
    }

    public void EndTurn() {
        if (!BoardController.Instance.DoneSwapingThing()) {
            return;
        }
        StartCoroutine(PlayerAttackAnimation());
        StartCoroutine(EnemyTakingDamage());
    }
    private IEnumerator PlayerAttackAnimation() {
        Player.Instance.AttackAnima();
        yield return new WaitForSeconds(Player.Instance.attackMoveDuration);
    }
    private IEnumerator EnemyTakingDamage() {
        int totalDamage = matchManager.GetAccumulatedDamage();
        _enemyUI.TakeDamage(totalDamage);
        // Debug.Log($"Enemy took {totalDamage} damage!");
        if (combatLogManager != null)
        {
            combatLogManager.AddLog($"Player dealt {totalDamage} damage!");
        }
        matchManager.ResetDamage(); 
        swapCounter.swapCount = 0;
        swapCounter.UpdateTurnText();
        yield return new WaitForSeconds(1f);
        PlayerMoved();
    }
    public void Matchable(){
        if (BoardController.Instance == null) {
            Debug.Log("Board controller missing");
            return;
        }
        BoardController.Instance.BeginSwap();
    }
    public void UnMatchable(){
        if (BoardController.Instance == null) {
            Debug.Log("Board controller missing");
            return;
        }
        BoardController.Instance.StopSwap();
    }

    public void CombatEnd() {
        if (isEnemyDie()) {
            Debug.Log("Player Win!");
            if (combatLogManager != null)
            {
                combatLogManager.AddLog("Player Win!");
                _currentStage.isComplete = true;
            }
        }
        else {
            Debug.Log("Player Lose!");
            if (combatLogManager != null)
            {
                combatLogManager.AddLog("Player Lose!");
            }
        }
        currentEnemy = null;
        StartCoroutine(EndingProcess());
    }

    private IEnumerator EndingProcess() {
        yield return new WaitForSeconds(1f);
        combatLogManager.AddLog("Ending.");
        yield return new WaitForSeconds(1f);
        combatLogManager.AddLog("Ending..");
        yield return new WaitForSeconds(1f);
        combatLogManager.AddLog("Ending...");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainScene");
    }

    public bool isEnemyDie() {
        return _enemyUI.currentHP <= 0;
    }
    public bool isPlayerDie() {
        return Player.Instance._currentHp <= 0; 
    }
    public void EnterStage(Stage stage) {
        _currentStage = stage;
    }
}
