using UnityEngine;

public class InstancingControl : MonoBehaviour
{
    public bool enableInstancing = true;

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            foreach (var mat in renderer.materials)
            {
                mat.enableInstancing = enableInstancing;
            }
        }
    }
}
