using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashVFXManager : MonoBehaviour
{
    public static SlashVFXManager Instance;
    public List<GameObject> slashVFXPrefab;

    private void Awake() {
        Instance = this;
    }

    public void PlaySlashEffect(Vector3 position, int i, bool isFacingLeft) {
        GameObject slash = Instantiate(slashVFXPrefab[i], position, Quaternion.identity);

        if (!isFacingLeft) {
            Vector3 scale = slash.transform.localScale;
            scale.x *= -1; 
            slash.transform.localScale = scale;
        }

        Destroy(slash, 0.6f); 
    }
}