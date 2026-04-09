using System.Collections.Generic;
using UnityEngine;

public class TargetRegistry : MonoBehaviour
{
    [SerializeField] private List<Transform> _targets = new();

    public IReadOnlyList<Transform> Targets => _targets;

    [ContextMenu("Auto Fill Targets From Scene (Editor Only)")]
    private void AutoFill()
    {
        _targets.Clear();
        var movers = FindObjectsByType<WaypointMoveProvider>(FindObjectsSortMode.None);

        foreach (var m in movers)
            _targets.Add(m.transform);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}