using System.Linq;
using UnityEngine;

public class TargetRegistry : MonoBehaviour
{
    [SerializeField] private CameraGroupFramer framer;

    private void Start()
    {
        var movers = FindObjectsOfType<TargetWaypointMover>();
        framer.SetTargets(movers.Select(m => m.transform).ToList());
    }
}