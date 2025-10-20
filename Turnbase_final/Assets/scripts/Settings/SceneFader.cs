using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// ตัวทำ Fade ครอบจอ (Canvas + Image) ใช้ได้ทุกซีน (DontDestroyOnLoad)
/// ใช้: StartCoroutine(SceneFader.Instance.FadeAndLoad(sceneName, duration, color));
/// </summary>
public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    Canvas _canvas;
    Image  _overlay;
    Coroutine _running;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // สร้าง Canvas+Image สำหรับทับจอ
        _canvas = new GameObject("ScreenFaderCanvas").AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = short.MaxValue; // ให้อยู่หน้าสุด
        DontDestroyOnLoad(_canvas.gameObject);

        var imgGO = new GameObject("Overlay");
        imgGO.transform.SetParent(_canvas.transform, false);
        _overlay = imgGO.AddComponent<Image>();
        _overlay.raycastTarget = false;

        var rt = _overlay.rectTransform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

        // เริ่มต้นโปร่งใส
        _overlay.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeAndLoad(string sceneName, float duration, Color color, bool async = true)
    {
        // กันสั่งซ้ำ
        if (_running != null) yield break;

        _running = StartCoroutine(FadeAndLoadRoutine(sceneName, duration, color, async));
        yield return _running;
        _running = null;
    }

    IEnumerator FadeAndLoadRoutine(string sceneName, float duration, Color color, bool async)
    {
        // Fade In (จากโปร่งใส -> ทึบ)
        yield return Fade(0f, 1f, duration, color);

        // Load scene
        if (async)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!op.isDone) yield return null;
        }
        else
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            yield return null;
        }

        // รอ 1 เฟรมให้ซีนใหม่ตั้งตัว
        yield return null;

        // Fade Out (จากทึบ -> โปร่งใส)
        yield return Fade(1f, 0f, duration, color);
    }

    IEnumerator Fade(float from, float to, float duration, Color color)
    {
        float t = 0f;
        // ตั้งสีโดยคง RGB ไว้ เปลี่ยน alpha ตาม from→to
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // ใช้ unscaled เพื่อไม่โดน Time.timeScale
            float a = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            _overlay.color = new Color(color.r, color.g, color.b, a);
            yield return null;
        }
        _overlay.color = new Color(color.r, color.g, color.b, to);
    }
}
