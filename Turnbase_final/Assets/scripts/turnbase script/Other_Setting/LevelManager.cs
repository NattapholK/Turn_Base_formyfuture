using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Slider _progressBar;
    private float _target;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("LoadScene Coroutine เริ่มทำงาน");
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }
    
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        _target = 0;
        _progressBar.value = 0;

        _loaderCanvas.SetActive(true);
        Debug.Log("หน้าโหลดแสดงแล้ว");

        yield return null;

        //เริ่มการโหลดซีน
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false; // ป้องกันไม่ให้ซีนใหม่ทำงานทันที

        while (scene.progress < 0.9f) // scene.progress จะหยุดที่ 0.9f เมื่อโหลดเสร็จแล้ว
        {
            _target = scene.progress;
            yield return null; 
        }

        _target = 1f;

        while (_progressBar.value < 0.99f)
        {
            yield return null;
        }

        //อนุญาตให้ซีนใหม่เริ่มทำงาน
        yield return new WaitForSeconds(1f);
        scene.allowSceneActivation = true;
        while (!scene.isDone)
        {
            yield return null;
        }


        //ซ่อนหน้าโหลด
        _loaderCanvas.SetActive(false);
        Debug.Log("ซ่อนหน้าโหลดแล้ว");
    }

    void Update()
    {
        _progressBar.value = Mathf.MoveTowards(_progressBar.value, _target, 3 * Time.deltaTime);
    }
}
