using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

public enum CursorType { Default, BlueArrow }

public class MouseController : MonoBehaviour
{
    private static MouseController _instance;

    public static MouseController Instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<MouseController>();
            return _instance;
        }
    }

    public delegate void ClickedHandler(Vector2 mousePosition);

    [System.Serializable]
    private struct CursorData
    {
        public CursorType Type;
        public Texture2D texture;
    }

    [SerializeField] private CursorData[] cursorDatas;

    public event ClickedHandler onLeftClick;
    public event ClickedHandler onRightClick;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) onLeftClick?.Invoke(Input.mousePosition);
        if (Input.GetMouseButtonDown(1)) onRightClick?.Invoke(Input.mousePosition);
    }

    public void ChangeCursor(CursorType newType)
    {
        if (newType == CursorType.Default)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            var cursorTexture = cursorDatas.First(x => x.Type == newType).texture;
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}
