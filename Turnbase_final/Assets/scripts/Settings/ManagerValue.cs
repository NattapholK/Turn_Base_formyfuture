using UnityEngine;

[CreateAssetMenu(fileName = "ManagerValue", menuName = "Scriptable Objects/ManagerValue")]
public class ManagerValue : ScriptableObject
{
    public float gameSound = 1f;
    public float playerSound = 1f;
    public float sensitiveValue = 1f;
    public int fpsIndex = 0;
    public int textureIndex = 0;
    public int qualityIndex = 0;
    public int LODIndex = 0;
    public float grassDistance = 0f;
    public bool isCompleteGame1 = false;
    public bool isCompleteGame2 = false;
    public bool isCompleteGame3 = false;
}
