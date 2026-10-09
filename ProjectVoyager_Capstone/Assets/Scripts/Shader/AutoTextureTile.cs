using UnityEngine;

public class AutoTextureTile : MonoBehaviour
{
    public float textureTilingX = 1.0f;
    public float textureTilingY = 1.0f;

    private Renderer _renderer;
    
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        UpdateTiling();
    }

    void UpdateTiling()
    {
        if (_renderer == null) return;

        Vector2 newTiling =
            new Vector2(textureTilingX * transform.localScale.x, textureTilingY * transform.localScale.y);

        _renderer.sharedMaterial.mainTextureScale = newTiling;
    }
}
