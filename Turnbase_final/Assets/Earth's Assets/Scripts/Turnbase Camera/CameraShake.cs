using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;


    private CinemachineBasicMultiChannelPerlin perlinNoise;
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
        perlinNoise = CameraManager.Instance.MainCameraObj.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    #region Camera Shake Methods

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(ShakeCameraCoroutine(duration, magnitude));
    }

    public void PresetShake_GeneralAttack()
    {
        ShakeCamera(CameraManager.Instance.cameraShakeSettings.generalAttackShakeDuration,
                     CameraManager.Instance.cameraShakeSettings.generalAttackShakeMagnitude);
    }

    public IEnumerator ShakeCameraCoroutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void ShakeCameraWithPerlinNoise(float duration, float amplitude, float frequency)
    {
        StartCoroutine(ShakeCameraWithPerlinNoiseCoroutine(duration, amplitude, frequency));
    }

    private IEnumerator ShakeCameraWithPerlinNoiseCoroutine(float duration, float amplitude, float frequency)
    {
        perlinNoise.AmplitudeGain = amplitude;
        perlinNoise.FrequencyGain = frequency;

        yield return new WaitForSeconds(duration);

        CameraManager.Instance.SetPerlinNoiseSettings();
    }

 

    #endregion


    #region Shake with Animation

    public void ExecuteCameraShakeWithThisAnimation(Animator anim)
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("player1_1_attack") || stateInfo.IsName("player1_1_skill"))
        {
            Debug.Log("Shake");
            //ShakeCameraWithPerlinNoise(0.5f, 1.5f, 0.3f);
            ShakeCamera(CameraManager.Instance.cameraShakeSettings.generalAttackShakeDuration,
                         CameraManager.Instance.cameraShakeSettings.generalAttackShakeMagnitude);
        }
        else if (stateInfo.IsName("player2_1_attack") || stateInfo.IsName("player2_1_skill"))
        {
            ShakeCamera(CameraManager.Instance.cameraShakeSettings.generalAttackShakeDuration,
                         CameraManager.Instance.cameraShakeSettings.generalAttackShakeMagnitude);
        }
        else if (stateInfo.IsName("player3_1_attack") || stateInfo.IsName("player3_1_skill"))
        {
            ShakeCamera(CameraManager.Instance.cameraShakeSettings.generalAttackShakeDuration,
                         CameraManager.Instance.cameraShakeSettings.generalAttackShakeMagnitude);
        }


    }

    #endregion
}
