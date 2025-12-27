using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Multiplayer.Center.Common;
using System;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Turnbase States")]
    [SerializeField] private CameraTurnbaseState _currentCameraState = CameraTurnbaseState.Idle;
    public CameraTurnbaseState currentCameraState
    {
        get => _currentCameraState;
        set
        {
            if (_currentCameraState != value)
            {
                _currentCameraState = value;
                CameraTurnbaseStateChanged(_currentCameraState);
            }
        }
    }

    [Header("Camera Target Positions")]
    public List<Transform> characterTargetPositionsByIndex;

    [Header("Camera Change Turn Settings")]
    public OffsetValueForEachCharacter[] offsetValuesForEachCharacter;

    [Header("Camera Settings")]
    public TurnbaseCameraSettings turnbaseCameraSettings;
    public PresetMovementSettings presetMovementSettings;
    public CameraShakeSettings cameraShakeSettings;

    [Header("Debug Settings")]
    public bool shouldCamereValuesUpdateRealTime = false;
    public bool debugMode = false;
    public int currentCharacterTurnIndex = 0;

    [Header("References")]
    public GameObject MainCameraObj;
    public GameObject OffsetObj;
    public GameObject CodeMovementObj;
    public GameObject CameraShakeObj;
    [Space]
    public Animator cameraAnimator;
    [Space]
    // public AttackSceneManager attackSceneManager;   
    public RotateTowardsTarget bossRotateTowardsTarget;


    //Private variables
    private CameraTurnbaseState previousCameraState;


    //Singleton Pattern
    public static CameraManager Instance; //Singleton pattern ทำเป็น instance จะได้เรียกใช้ method ง่ายๆ
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeValueSettings();
    }

    private void OnValidate() //Idk what for.
    {
        currentCameraState = _currentCameraState;
    }

    private void Update()
    {
        if (shouldCamereValuesUpdateRealTime)
        {
            OffsetObj.transform.localPosition = offsetValuesForEachCharacter[currentCharacterTurnIndex].offsetPosition;
            SetCinemechineLookAtOffsetValue();
            SetPerlinNoiseSettings();
        }
    }


    //ระบบกล้องใหม่ สำหรับตอนสลับเทิร์น
    public void CameraChangeTurnTransition(int currentTurnIndex, bool isAnimation = true)
    {
        currentCameraState = CameraTurnbaseState.Idle;
        currentCharacterTurnIndex = currentTurnIndex;
        SetCinemechineLookAtOffsetValue();
        Transform targetPosition = GetCharacterTargetPosition(currentTurnIndex);

        CodeMovementObj.transform.localPosition = Vector3.zero;

        transform.position = targetPosition.position;
        OffsetObj.transform.localPosition = offsetValuesForEachCharacter[currentTurnIndex].offsetPosition;

        if(isAnimation) cameraAnimator.SetTrigger("changeTurn");
    }










    //Methods ขยับกล้องผ่านโค้ดบางอันใช้แบบสถานการณ์เฉพาะ บางอันก็ยืดหยุ่น
    #region Camera Action Presets

    public void PresetMovement_SelectingSkillButton()
    {
        //Vector3 SelectingSkillOffset = new Vector3(0.3f, 0.5f, 0f);
        Vector3 targetPosition = offsetValuesForEachCharacter[currentCharacterTurnIndex].offsetPosition + presetMovementSettings.selectingSkillButtonOffset;
        LerpOffsetPosition(targetPosition, presetMovementSettings.selectingSkillButtonLerpDuration);
    }

    public void PresetMovement_ResetFromSelectingSkillButton()
    {
        Vector3 targetPosition = offsetValuesForEachCharacter[currentCharacterTurnIndex].offsetPosition;
        LerpOffsetPosition(targetPosition, 0.2f);
    }




    #endregion




    #region Camera State Methods

    private void CameraTurnbaseStateChanged(CameraTurnbaseState newState)
    {
        switch (newState)
        {
            case CameraTurnbaseState.Idle:
                /*if(previousCameraState == CameraTurnbaseState.SelectingFirstSkillButton || previousCameraState == CameraTurnbaseState.SelectingSecondSkillButton)
                {
                    PresetMovement_ResetFromSelectingSkillButton();
                }*/
                break;

            case CameraTurnbaseState.SelectingFirstSkillButton:
                PresetMovement_SelectingSkillButton();
                break;

            case CameraTurnbaseState.SelectingSecondSkillButton:
                PresetMovement_SelectingSkillButton();
                break;

            default:
                Debug.LogWarning("Unhandled camera state: " + newState);
                break;
        }

        previousCameraState = newState;

    }


    #endregion

































    #region Initialization Methods
    public void InitializeValueSettings()
    {
        SetPerlinNoiseSettings();
    }


    #endregion

























    #region Helper Methods


    //Method นี้จะ return ตำแหน่งของ transform ตัวละครในเทิร์นนั้นๆโดยใช้ index จาก currentTurnIndex ใน AttackSceneManager.cs
    public Transform GetCharacterTargetPosition(int index)
    {
        Debug.Log("Getting target position for index: " + index);
        if (index >= 0 && index < characterTargetPositionsByIndex.Count)
        {
            return characterTargetPositionsByIndex[index];
        }
        else
        {
            Debug.LogError("Index out of range for character target positions.");
            return null;
        }
    }

    //Method นี้จะ set ค่า offset ให้กับ component Cinemachine Hard Look At ของตัวกล้อง ปรับค่าตรง cinemachineLookAtOffset ใน inspector ได้เลย ถ้าปรับตรง component จะไม่เห็นผล โดนเซ็ตทับ
    public void SetCinemechineLookAtOffsetValue()
    {
        MainCameraObj.GetComponent<CinemachineHardLookAt>().LookAtOffset = offsetValuesForEachCharacter[currentCharacterTurnIndex].cinemachineLookAtOffset;
    }

    public void SetPerlinNoiseSettings()
    {
        CinemachineBasicMultiChannelPerlin perlin = MainCameraObj.GetComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.AmplitudeGain = turnbaseCameraSettings.amplitudeGain;
        perlin.FrequencyGain = turnbaseCameraSettings.frequencyGain;
    }

    #endregion


    #region Lerp Value Methods

    public void LerpOffsetPosition(Vector3 targetPosition, float duration)
    {
        if (_offsetLerpCoroutine != null)
        {
            StopCoroutine(_offsetLerpCoroutine);
            _offsetLerpCoroutine = null;
        }
        _offsetLerpCoroutine = StartCoroutine(LerpOffsetPositionCoroutine(targetPosition, duration));
    }

    private Coroutine _offsetLerpCoroutine;

    private System.Collections.IEnumerator LerpOffsetPositionCoroutine(Vector3 targetPosition, float duration)
    {
        if (OffsetObj == null)
            yield break;

        Vector3 startPos = OffsetObj.transform.localPosition;
        float elapsed = 0f;
        while (elapsed < Mathf.Max(0.0001f, duration))
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - Mathf.Pow(1f - t, 3); // Cubic ease out
            OffsetObj.transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }
        OffsetObj.transform.localPosition = targetPosition;
        _offsetLerpCoroutine = null;
    }



    #endregion

}

[System.Serializable]
public class OffsetValueForEachCharacter
{
    public Vector3 offsetPosition;
    public Vector3 cinemachineLookAtOffset;
}

[System.Serializable]
public class TurnbaseCameraSettings
{
    [Header("Perlin Noise Settings")]
    public float amplitudeGain;
    public float frequencyGain;

}

[System.Serializable]
public class PresetMovementSettings
{
    [Header("Selecting Skill Button Settings")]
    public Vector3 selectingSkillButtonOffset;
    public float selectingSkillButtonLerpDuration;
}

[System.Serializable]
public class CameraShakeSettings
{
    [Header("General Attack")]
    public float generalAttackShakeDuration;
    public float generalAttackShakeMagnitude;
}


[System.Serializable]
public enum CameraTurnbaseState
{
    Idle,
    SelectingFirstSkillButton,
    SelectingSecondSkillButton,
}
