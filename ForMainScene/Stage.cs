using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public enum StageType { Normal, Boss }

[CreateAssetMenu(fileName = "NewStage", menuName = "ScriptableObjects/Stage")]
public class Stage : ScriptableObject
{
    public string stageName;
    public Image stageImage;
    public Enemy enemies;
    public String sceneName;
    public StageType stageType;
    public bool isComplete = false;

    public bool IsBossStage => stageType == StageType.Boss;
}
