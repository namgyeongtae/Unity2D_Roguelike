using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<UIManager>();
            }
            return instance;
        }
    }

    [SerializeField] private CanvasManager canvasManager;

    private void Awake()
    {
        AddPanel<UIPlayer>("UIPlayer");
    }

    public T AddPanel<T>(string panelName) where T : CanvasPanel
    {
        return canvasManager.AddPanel<T>(panelName);
    }

    public void RemovePanel<T>(string panelName) where T : CanvasPanel
    {
        canvasManager.RemovePanel<T>(panelName);
    }

    public T GetPanel<T>(string panelName) where T : CanvasPanel
    {
        return canvasManager.GetPanel<T>(panelName);
    }
}
