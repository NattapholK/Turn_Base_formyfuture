using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RotateTowardsTarget : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The target Transform to rotate towards.")]
    public Transform target;
    //public Transform defaultTarget;

    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second. Set to 0 for instant rotation.")]
    public float rotationSpeed = 360f;

    [Tooltip("Offset to apply after looking at the target (in Euler angles).")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("If true, only rotate on the Y axis (useful for 2.5D or top-down).")]
    public bool onlyY = false;

    [Header("Enable/Disable Rotation")]
    [Tooltip("If false, this object will not rotate towards the target.")]
    public bool enableRotation = true;

    void Update()
    {
        if (!enableRotation) return;
        if (target == null) return;

        // Calculate direction to target
        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude < 0.0001f) return; // Avoid zero direction

        Quaternion targetRotation;

        if (onlyY)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) return;
            targetRotation = Quaternion.LookRotation(direction);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(direction);
        }

        // Apply offset
        targetRotation *= Quaternion.Euler(rotationOffset);

        // Rotate smoothly or instantly
        if (rotationSpeed > 0f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }

    public void AssignTargetAndEnable(Transform newTarget)
    {
        target = newTarget;
        enableRotation = true;
    }

    public void ResetTargetAndLerpRotationToZero()
    {
        target = null;
        enableRotation = false;

        StartCoroutine(LerpRotationToZero());
    }

    public IEnumerator LerpRotationToZero()
    {
    
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);
        float duration = 1f; // Duration of the lerp
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}