using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private List<CanvasPanel> panelList = new List<CanvasPanel>();

    public T AddPanel<T>(string panelName) where T : CanvasPanel
    {
        string path = "";

        if (string.IsNullOrEmpty(panelName))
            path = typeof(T).Name;
        else
            path = panelName;

        var obj = Resources.Load<T>($"UI/{path}");

        if (obj == null)
        {
            Debug.LogError($"Panel {path} not found");
            return null;
        }

        if (panelList.Contains(obj))
        {
            Debug.LogError($"Panel {path} already exists");
            return null;
        }

        panelList.Add(obj);

        var panel = Instantiate(obj, transform);
        panel.transform.localScale = Vector3.one;
        panel.transform.localPosition = Vector3.zero;

        panel.Initialize();

        return panel;
    }

    public T GetPanel<T>(string panelName) where T : CanvasPanel
    {
        string objName = string.IsNullOrEmpty(panelName) ? typeof(T).Name : panelName;

        var panel = panelList.FirstOrDefault(x => x.GetType().Name == objName);

        if (panel == null)
        {
            Debug.LogError($"Panel {objName} not found");
            return null;
        }

        return panel as T;
    }

    public void RemovePanel<T>(string panelName) where T : CanvasPanel
    {
        var obj = panelList.FirstOrDefault(x => x.GetType().Name == panelName);

        if (obj == null)
        {
            Debug.LogError($"Panel {panelName} not found");
            return;
        }

        panelList.Remove(obj);
        Destroy(obj.gameObject);
    }
}
