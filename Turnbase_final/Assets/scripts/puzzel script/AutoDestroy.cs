// File: AutoDestroy.cs
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float seconds = 5f;
    void Start()
    {
        if (seconds > 0f) Destroy(gameObject, seconds);
    }
}
