using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light sunLight;  // Assign your Directional Light here
    [SerializeField] private float dayDuration = 120f; // Total time for a full day-night cycle (in seconds)

    private float timeElapsed = 0f;

    void FixedUpdate()
    {
        timeElapsed += Time.deltaTime;
        float dayProgress = (timeElapsed / dayDuration) % 1; // Normalized [0,1] cycle

        // Rotate Sun (0° = Midnight, 180° = Noon, 360° = Midnight)
        float sunAngle = dayProgress * 360f;
        sunLight.transform.rotation = Quaternion.Euler(sunAngle - 90, 0, 0);

        // Change Light Intensity
        sunLight.intensity = Mathf.Clamp01(Mathf.Sin(dayProgress * Mathf.PI));

        // Change Light Color (optional)
        sunLight.color = Color.Lerp(Color.blue, Color.yellow, Mathf.Clamp01(Mathf.Sin(dayProgress * Mathf.PI)));

        // Change Ambient Lighting (optional)
        RenderSettings.ambientIntensity = Mathf.Clamp01(sunLight.intensity);

        // Add Skybox or Weather Effects Here (optional)
    }
}
