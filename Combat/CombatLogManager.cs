using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CombatLogManager : MonoBehaviour
{
    public TextMeshProUGUI combatLogText; 
    public int maxLines = 6; // Maximum lines before old logs disappear

    private Queue<string> logEntries = new Queue<string>();
    void Awake()
    {
        GameController.Instance.combatLogManager = this;
    }

    public void AddLog(string message)
    {
        logEntries.Enqueue(message);
        if (logEntries.Count > maxLines)
        {
            logEntries.Dequeue();
        }
        combatLogText.text = string.Join("\n", logEntries);
    }
}
