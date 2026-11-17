using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneIntroOverlay : MonoBehaviour
{
    [SerializeField] 
    private Image overlayImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HandleSceneEntryOverlay();
    }

    // Update is called once per frame
    void Update()
    {
        if (overlayImage.color == Color.clear)
        {
            Destroy(gameObject);
        }
    }

    void HandleSceneEntryOverlay()
    {
        StartCoroutine(LerpOverlayColor(Color.clear, 1.5f));
    }
    
    IEnumerator LerpOverlayColor(Color targColor, float duration)
    {
        Color startColor = overlayImage.color;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float transition = Mathf.Clamp01(elapsed / duration);
            
            overlayImage.color = Color.Lerp(startColor, targColor, transition);
            yield return null;
        }

        overlayImage.color = targColor;
    }
}
