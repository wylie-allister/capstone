using System.Collections;
using PSX;
using UnityEngine;
using UnityEngine.UI;

public class SceneIntroOverlay : MonoBehaviour
{
    [SerializeField] 
    private Image overlayImage;
    
    [SerializeField]
    private PixelationController pixelationController;

    private Vector2 finalPixelCount;
    private bool isPixelsAtTarget = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        finalPixelCount = new Vector2(pixelationController.widthPixelation, pixelationController.heightPixelation);
        pixelationController.widthPixelation = 16;
        pixelationController.heightPixelation = 8;
        HandleSceneEntryOverlay();

    }

    // Update is called once per frame
    void Update()
    {
        if (overlayImage.color == Color.clear && isPixelsAtTarget)
        {
            Destroy(gameObject);
        }
    }

    void HandleSceneEntryOverlay()
    {
        if (pixelationController != null)
            StartCoroutine(LerpPixels(finalPixelCount, 1.25f));
        
        StartCoroutine(LerpOverlayColor(Color.clear, 1.5f));
        

    }

    IEnumerator LerpPixels(Vector2 targetPixelCount, float duration)
    {
        Debug.Log(pixelationController.widthPixelation);
        Vector2 startCount = new Vector2(pixelationController.widthPixelation, pixelationController.heightPixelation);
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            float transition = Mathf.Clamp01(elapsed / duration);
            
            pixelationController.widthPixelation = Mathf.Lerp(startCount.x, targetPixelCount.x, transition);
            pixelationController.heightPixelation = Mathf.Lerp(startCount.y, targetPixelCount.y, transition);
            yield return null;
        }
        
        pixelationController.widthPixelation = targetPixelCount.x;
        pixelationController.heightPixelation = targetPixelCount.y;
        isPixelsAtTarget = true;
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
