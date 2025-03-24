using UnityEngine;
using UnityEngine.UI;
using Match3;

public class SwapCounter : MonoBehaviour {
    public int swapCount = 0;
    private int swapValue = 2;
    [SerializeField] Text text;
    void Awake() {
        GameController.Instance.swapCounter = this;
    }
    void Start() {
        UpdateTurnText();
    }
    private void OnEnable() {
        EventBus.OnBlockSwapped += IncrementSwapCount;
    }

    private void OnDisable() {
        EventBus.OnBlockSwapped -= IncrementSwapCount;
    }

    private void IncrementSwapCount() {
        swapCount++;
        UpdateTurnText();
        if (swapCount == swapValue) {
            GameController.Instance.UnMatchable();
        }
    }
    public void UpdateTurnText() {
        text.text = "Turn: " + (swapValue-swapCount);
    }
}
