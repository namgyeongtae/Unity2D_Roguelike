using UnityEngine;

public class KeyInputController : MonoBehaviour
{
    public delegate void KeyInputHandler(Vector2 inputDir);

    private static KeyInputController _instance;
    public static KeyInputController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<KeyInputController>();
            }
            return _instance;
        }
    }


    public event KeyInputHandler onKeyInputDown;
    private bool _isKeyDown = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       OnKeyDown();
       OnKeyUp();
    }

    private void OnKeyDown()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            onKeyInputDown?.Invoke(Vector2.up);
        else if (Input.GetKey(KeyCode.LeftArrow))
            onKeyInputDown?.Invoke(Vector2.left);
        else if (Input.GetKey(KeyCode.DownArrow))
            onKeyInputDown?.Invoke(Vector2.down);
        else if (Input.GetKey(KeyCode.RightArrow))
            onKeyInputDown?.Invoke(Vector2.right);
    }

    private void OnKeyUp()
    {
        //if (!_isKeyDown)
        //    return;

        //if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.LeftArrow) 
        //|| Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        //{
        //    _isKeyDown = false;
        //}
    }
}
