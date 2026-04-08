using UnityEngine;

public interface IMoveVectorProvider
{
    Vector3 GetDesiredHorizontalVelocity();
    
    bool ShouldRotate { get; }

    Vector3 LookDirection { get; }
    
    float MoveIntensity01 { get; }
}