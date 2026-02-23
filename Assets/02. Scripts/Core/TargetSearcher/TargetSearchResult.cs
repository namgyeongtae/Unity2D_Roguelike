using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct TargetSearchResult
{
    public readonly IReadOnlyList<GameObject> targets;
    public readonly IReadOnlyList<Vector3> positions;

    public TargetSearchResult(GameObject[] targets)
        => (this.targets, positions) = (targets, targets.Select(x => x.transform.position).ToArray());
    
    public TargetSearchResult(Vector3[] positions)
        => (targets, this.positions) = (Array.Empty<GameObject>(), positions);
}
