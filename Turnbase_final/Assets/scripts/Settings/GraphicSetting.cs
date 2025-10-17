using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GraphicSetting : MonoBehaviour
{
    private class LODInfo
    {
        public LODGroup lodGroup;
        public float[] originalValues;
    }

    private List<LODInfo> lodInfos = new List<LODInfo>();

    [Header("DropDown")]
    public TMP_Dropdown FPSDropdown;
    public TMP_Dropdown textureDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown LODDropdown;

    [Header("Slider")]
    public Slider grassDistanceSlider;


    [Header("Scriptable Object")]
    [Tooltip("เอามาจาก scripts/Settings/ManagerValue.cs")]
    public ManagerValue managerValue;

    [Header("Terrain")]
    public Terrain terrain;


    private List<int> fpsList = new List<int> { 30, 60, 120, 144 };
    private List<int> textureDList = new List<int> { 100, 5, 0 };
    private List<float> LODList = new List<float> { 2f, 1.5f, 1f };
    void Start()
    {
        // เก็บค่าเริ่มต้น
        LODGroup[] allLods = Object.FindObjectsByType<LODGroup>(FindObjectsSortMode.None);
        foreach (LODGroup lodGroup in allLods)
        {
            if (lodGroup == null) continue;
            LOD[] lods = lodGroup.GetLODs();
            float[] originals = new float[lods.Length];
            for (int i = 0; i < lods.Length; i++)
                originals[i] = lods[i].screenRelativeTransitionHeight;

            lodInfos.Add(new LODInfo { lodGroup = lodGroup, originalValues = originals });
        }

        QualitySettings.vSyncCount = 0; // ปิด V-Sync

        FPSDropdown.value = managerValue.fpsIndex;
        textureDropdown.value = managerValue.textureIndex;
        qualityDropdown.value = managerValue.qualityIndex;
        LODDropdown.value = managerValue.LODIndex;
        grassDistanceSlider.value = managerValue.grassDistance;

        OnFPSDropdownChanged(FPSDropdown.value);
        OnTextureDropdownChanged(textureDropdown.value);
        OnQualityDropdownChanged(qualityDropdown.value);
        OnLODDropdownChanged(LODDropdown.value);
        OnGrassDistanceSliderChanged(grassDistanceSlider.value);

        FPSDropdown.onValueChanged.AddListener(OnFPSDropdownChanged); //รับค่า index มา
        textureDropdown.onValueChanged.AddListener(OnTextureDropdownChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityDropdownChanged);
        LODDropdown.onValueChanged.AddListener(OnLODDropdownChanged);
        grassDistanceSlider.onValueChanged.AddListener(OnGrassDistanceSliderChanged);
    }

    private void OnQualityDropdownChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
        managerValue.qualityIndex = index;
    }
    
    private void OnLODDropdownChanged(int index)
    {
        AdjustAllLODs(LODList[index]);
        managerValue.LODIndex = index;
    }

    private void OnFPSDropdownChanged(int index)
    {
        Application.targetFrameRate = fpsList[index];
        managerValue.fpsIndex = index;
    }

    private void OnTextureDropdownChanged(int index)
    {
        QualitySettings.globalTextureMipmapLimit = textureDList[index];
        managerValue.textureIndex = index;
    }

    private void OnGrassDistanceSliderChanged(float value)
    {
        if (terrain != null){
            terrain.detailObjectDistance = 1000f * value;
        }
        managerValue.grassDistance = value;
    }

    public void AdjustAllLODs(float factor)
    {
        foreach (LODInfo info in lodInfos)
        {
            if (info.lodGroup == null) continue;

            LOD[] lods = info.lodGroup.GetLODs();
            for (int i = 0; i < lods.Length; i++)
            {
                lods[i].screenRelativeTransitionHeight = info.originalValues[i] * factor;
            }

            // ตรวจสอบลำดับ
            for (int i = 1; i < lods.Length; i++)
            {
                if (lods[i].screenRelativeTransitionHeight >= lods[i-1].screenRelativeTransitionHeight)
                    lods[i].screenRelativeTransitionHeight = lods[i-1].screenRelativeTransitionHeight * 0.99f;
            }

            info.lodGroup.SetLODs(lods);
            info.lodGroup.RecalculateBounds();
        }
    }

}
