using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;


public class StageUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI biomeNameText;
    [SerializeField] private TextMeshProUGUI biomeShadowNameText;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private Button previousStageButton;

    [SerializeField] private List<Biome> biomes;
    private int currentBiomeIndex = 0;

    [SerializeField] private GameObject[] combatBoxes; // 3 UI boxes to show enemies

    void Start()
    {
        if (biomes == null || biomes.Count == 0)
        {
            Debug.LogError("No biome assigned to StageUIManager.");
            return;
        }

        UpdateStageUI();

        nextStageButton.onClick.AddListener(SwitchToNextStage);
        previousStageButton.onClick.AddListener(SwitchToPreviousStage);
    }

    private void SwitchToNextStage()
    {
        if (currentBiomeIndex < biomes.Count - 1)
        {
            currentBiomeIndex++;
            UpdateStageUI();
        }
        else
        {
            currentBiomeIndex = 0;
        }
    }

    private void SwitchToPreviousStage()
    {
        if (currentBiomeIndex > 0)
        {
            currentBiomeIndex--;
            UpdateStageUI();
        }
        else
        {
            currentBiomeIndex = biomes.Count - 1;
        }
    }

    private void UpdateStageUI()
    {
        Biome currentBiome = GetCurrentBiome();
        biomeNameText.text = currentBiome.biomeName;
        biomeShadowNameText.text = currentBiome.biomeName;

        // Show enemies in combat boxes
        for (int i = 0; i < combatBoxes.Length; i++)
        {
            if (i < currentBiome.stages.Count && !currentBiome.stages[i].isComplete)
            {
                combatBoxes[i].SetActive(true);
                // Assume each combat box has a child Text component to show enemy name
                TextMeshProUGUI stageText = combatBoxes[i].GetComponentInChildren<TextMeshProUGUI>();
                stageText.text = currentBiome.stages[i].stageName;
                // Image stageImage = combatBoxes[i].GetComponentInChildren<Image>();
                // stageImage.sprite = currentBiome.stages[i].stageImage.sprite;

                // Remove old listeners to avoid stacking
                Button boxButton = combatBoxes[i].GetComponent<Button>();
                boxButton.onClick.RemoveAllListeners();

                // Add new listener for scene loading
                int stageIndex = i; // Capture current loop index
                boxButton.onClick.AddListener(() => LoadStageScene(currentBiome.stages[stageIndex].sceneName, currentBiome.stages[stageIndex].enemies));
                boxButton.onClick.AddListener(() => GameController.Instance.EnterStage(currentBiome.stages[stageIndex]));
            }
            else
            {
                combatBoxes[i].SetActive(false);
            }
        }

        Debug.Log($"Switched to stage: {currentBiome.biomeName}");
    }

    private Biome GetCurrentBiome()
    {
        return biomes[currentBiomeIndex];
    }

    private void LoadStageScene(string sceneName, Enemy enemie)
    {
        Debug.Log("Loading scene: " + sceneName);
        GameController.Instance.SetCurrentEnemie(enemie);
        SceneManager.LoadScene(sceneName);
    }
}
