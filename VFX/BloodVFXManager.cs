using UnityEngine;
using System.Collections.Generic;

public class BloodVFXManager : MonoBehaviour {
    public static BloodVFXManager Instance;
    public List<GameObject> bloodVFXPrefab; // Assign in Inspector

    private void Awake() {
        Instance = this;
    }

    public void PlayBloodEffect(Vector3 position) {
        int temp = Random.Range(2, 5);
        while ((temp -= 1) >= 0) {
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.5f, 0.5f), 
                Random.Range(-1f, 1f), 
                0f 
            );
            GameObject blood = Instantiate(bloodVFXPrefab[0], position + randomOffset, Quaternion.identity);
            Destroy(blood, 0.6f); 
        }
    }
}

