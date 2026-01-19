using UnityEngine;

public abstract class CanvasPanel : MonoBehaviour
{
    public abstract void Initialize();
    public abstract void Release();

    void Start()
    {
        Initialize();
    }

    void OnDisable()
    {
        Release();
    }

    void OnDestroy()
    {
        Release();
        Destroy(gameObject);
    }
}
