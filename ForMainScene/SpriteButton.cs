using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject panelToShow; 
    private Vector3 originalScale;
    [SerializeField] private GameObject outlineObject; 
    private SpriteRenderer outlineRenderer;

    void Start()
    {
        originalScale = transform.localScale;
        if (outlineObject != null)
        {
            outlineRenderer = outlineObject.GetComponent<SpriteRenderer>();
            outlineRenderer.enabled = false; 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = true; // Show outline on hover
        }
        transform.localScale = originalScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outlineRenderer != null)
        {
            outlineRenderer.enabled = false; // Hide outline when exiting hover
        }
        transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (panelToShow != null)
        {
            panelToShow.SetActive(true); // Show the UI panel
        }
    }
}
