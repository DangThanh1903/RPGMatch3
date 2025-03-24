
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    private Button button;
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(GameController.Instance.EndTurn);
    }
}
