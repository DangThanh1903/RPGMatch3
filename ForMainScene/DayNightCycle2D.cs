using UnityEngine;

public class DayNightCycle2D : MonoBehaviour
{
    [SerializeField] private Transform sun; // Assign Sun sprite
    [SerializeField] private Transform moon; // Assign Moon sprite
    [SerializeField] private Transform centerPoint; // The center of rotation (middle of the sky)

    [SerializeField] private float cycleDuration = 20f; // Full day-night cycle duration (seconds)
    private float timeElapsed;

    [SerializeField] private float xRadius = 6f; // Horizontal radius (wider)
    [SerializeField] private float yRadius = 3f; // Vertical radius (narrower)

    void Update()
    {
        timeElapsed += Time.deltaTime;
        float angle = (timeElapsed / cycleDuration) * 360f;
        float radians = angle * Mathf.Deg2Rad;

        // Move the Sun in an elliptical path
        sun.position = new Vector3(
            centerPoint.position.x + Mathf.Cos(radians) * xRadius,
            centerPoint.position.y + Mathf.Sin(radians) * yRadius,
            sun.position.z
        );

        if (moon == null) return; // Skip if Moon is not assigned

        // Move the Moon in the opposite elliptical path
        moon.position = new Vector3(
            centerPoint.position.x + Mathf.Cos(radians + Mathf.PI) * xRadius,
            centerPoint.position.y + Mathf.Sin(radians + Mathf.PI) * yRadius,
            moon.position.z
        );
    }
}
