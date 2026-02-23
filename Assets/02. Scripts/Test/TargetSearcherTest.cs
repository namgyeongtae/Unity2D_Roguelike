using UnityEngine;

public class TargetSearcherTest : MonoBehaviour
{
    [SerializeField] private float scale = 1f;
    [SerializeField] private TargetSearcher targetSearcher;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            scale -= 0.1f;
            targetSearcher.Scale = scale;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            scale += 0.1f;
            targetSearcher.Scale = scale;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            targetSearcher.ShowIndicator(gameObject);
            targetSearcher.SelectTarget(GetComponent<Entity>(), gameObject, (_, selectionResult) => 
            {
                targetSearcher.HideIndicator();

                switch (selectionResult.resultMessage)
                {
                    case SearchResultMessage.Fail:
                        Debug.Log("<color=red>Selection Fail</color>");
                        break;
                    
                    case SearchResultMessage.OutOfRange:
                        Debug.Log("<color=yellow>Out Of Range</color>");
                        break;
                    
                    default:
                        if (selectionResult.selectedTarget)
                            Debug.Log($"<color=green>Selected Target: {selectionResult.selectedTarget.name}</color>");
                        else
                            Debug.Log($"<color=green>Selected Position: {selectionResult.selectedPosition}</color>");
                        
                        var searchResult = targetSearcher.SearchTargets(GetComponent<Entity>(), gameObject);
                        if (searchResult.targets.Count > 0)
                        {
                            foreach (var target in searchResult.targets)
                                Debug.Log($"<color=magenta>Searched Target: {target.name}</color>");
                        }
                        else
                        {
                            foreach (var targetPosition in searchResult.positions)
                                Debug.Log($"<color=magenta>Searched Position: {targetPosition}</color>");
                        }
                        break;
                }
            });
        }
    }
}
