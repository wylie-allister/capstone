using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneExitOverlay : MonoBehaviour
{
    [SerializeField] 
    private Image overlayImage;

    public bool toggle = false;

    public bool hasFinished = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (toggle)
        {
            HandleSceneExitOverlay();
            toggle = false;
        }
    }

    void HandleSceneExitOverlay()
    {
        StartCoroutine(LerpOverlayColor(Color.black, 1.5f));
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
        hasFinished = true;
    }
}