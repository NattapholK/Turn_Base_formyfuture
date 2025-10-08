using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{


    [Header("References")]
    public GameObject MainCameraObj;
    public GameObject OffsetObj;
    public GameObject CodeMovementObj;
    public GameObject CameraShakeObj;
    [Space]
    public Animator cameraAnimator;
    [Space]
    public AttackSceneManager attackSceneManager;


    [Header("Camera Target Positions")]
    public List<Transform> characterTargetPositionsByIndex;

    [Header("Camera Change Turn Settings")]
    public OffsetValueForEachCharacter[] offsetValuesForEachCharacter;

    [Header("Debug Settings")]
    public bool shouldCameraPositionUpdateRealTime = false;
    public bool debugMode = false;
    public int currentCharacterTurnIndex = 0;


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

    private void Update()
    {
        if (shouldCameraPositionUpdateRealTime)
        {
            OffsetObj.transform.localPosition = offsetValuesForEachCharacter[currentCharacterTurnIndex].offsetPosition;
            SetCinemechineLookAtOffsetValue();
        }
    }


    //ระบบกล้องใหม่ สำหรับตอนสลับเทิร์น
    public void CameraChangeTurnTransition(int currentTurnIndex)
    {
        currentCharacterTurnIndex = currentTurnIndex;
        SetCinemechineLookAtOffsetValue();
        Transform targetPosition = GetCharacterTargetPosition(currentTurnIndex);

        CodeMovementObj.transform.localPosition = Vector3.zero;

        transform.position = targetPosition.position;
        OffsetObj.transform.localPosition = offsetValuesForEachCharacter[currentTurnIndex].offsetPosition;

        cameraAnimator.SetTrigger("changeTurn");
    }




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
    
    #endregion

}

[System.Serializable]
public class OffsetValueForEachCharacter
{
    public Vector3 offsetPosition;
    public Vector3 cinemachineLookAtOffset;
}
