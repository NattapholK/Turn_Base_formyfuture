using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public GameObject backgroundUI;
    public Transform parentPanel;
    public List<GameObject> turnPlayerUIPrefabList;
    public GameObject turnEnemyUIPrefab;

    private int LastTurn;
    private float LastOffset;
    private StatusSystemScript statusScript;
    private List<UIUnit> uiUnits = new List<UIUnit>();
    private List<UIUnit> uiInScene = new List<UIUnit>();
    private float bgUIHight;
    private RectTransform bgUIRecttranform;
    private AttackSceneManager attackSceneManager;
    void Awake()
    {
        attackSceneManager = GetComponent<AttackSceneManager>();
        statusScript = GetComponent<StatusSystemScript>();
    }
    void Start()
    {

        for (int i = 0; i < turnPlayerUIPrefabList.Count; i++)
        {
            uiUnits.Add(new UIUnit
            {
                speed = statusScript.speedPlayerList[i],
                UI = turnPlayerUIPrefabList[i],
                index = i
            });
        }
        uiUnits.Add(new UIUnit
        {
            speed = statusScript.speedBoss,
            UI = turnEnemyUIPrefab,
            index = uiUnits.Count
        });

        uiUnits = uiUnits.OrderByDescending(u => u.speed).ToList();

        bgUIRecttranform = backgroundUI.GetComponent<RectTransform>();
        bgUIHight = bgUIRecttranform.rect.height;
        CreateTurnIcons();
    }
    public void CreateTurnIcons()
    {
        float offsetY = 0f;

        for (int i = 0; offsetY < bgUIHight; i++)
        {
            if (i >= uiUnits.Count)
            {
                i = 0;
            }
            LastTurn = i + 1;

            GameObject icon = Instantiate(uiUnits[i].UI, parentPanel);
            RectTransform rt = icon.GetComponent<RectTransform>();

            float height = rt.rect.height * rt.localScale.y;

            // จัดเรียงแบบ top-down
            rt.anchoredPosition = new Vector2(0, -offsetY);

            LastOffset = offsetY;
            offsetY += height;

            uiInScene.Add(new UIUnit
            {
                speed = uiUnits[i].speed,
                UI = icon,
                index = i
            });

            if (offsetY + height > bgUIHight)
            {
                break;
            }
        }
    }
    public IEnumerator DeleteTurnIcon()
    {
        RectTransform firstUI = uiInScene[0].UI.GetComponent<RectTransform>();
        float height = firstUI.rect.height * firstUI.localScale.y;
        yield return StartCoroutine(SlideAndFade(uiInScene[0].UI));

        Destroy(uiInScene[0].UI);
        uiInScene.Remove(uiInScene[0]);

        foreach (UIUnit UI in uiInScene)
        {
            StartCoroutine(goUpUI(UI.UI, height));
        }
        AddTurnIcon();
    }

    void AddTurnIcon()
    {
        if (LastTurn >= uiUnits.Count)
        {
            LastTurn = 0;
        }
        while (attackSceneManager.allUnits[LastTurn].isdied)
        {
            LastTurn++;
            if (LastTurn >= uiUnits.Count)
            {
                LastTurn = 0;
            }
        }
        GameObject icon = Instantiate(uiUnits[LastTurn].UI, parentPanel);
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -LastOffset);

        uiInScene.Add(new UIUnit
        {
            speed = uiUnits[LastTurn].speed,
            UI = icon,
            index = LastTurn
        });
        LastTurn++;
    }

    public IEnumerator ScaleUI(RectTransform UI, string scaleType)
    {
        float duration = 0.1f;
        Vector3 startScale = UI.localScale;
        float multipleScale = 1;
        switch (scaleType)
        {
            case "up":
                multipleScale *= 1.2f;
                break;
            case "down":
                multipleScale /= 1.2f;
                break;
            default:
                break;
        }

        Vector3 endScale = startScale * multipleScale;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration; // ค่อย ๆ เพิ่มค่า t (0 → 1)
            UI.localScale = Vector3.Lerp(startScale, endScale, t); // ค่อย ๆ ขยาย
            yield return null;
        }

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator CheckDiedPlayerIcon()
    {

        List<int> PlayerDieIndex = new List<int>();

        foreach (BattleUnit unit in attackSceneManager.allUnits)
        {
            if (unit.isdied)
            {
                PlayerDieIndex.Add(attackSceneManager.allUnits.IndexOf(unit));
                Debug.Log("attackSceneManager.allUnits.IndexOf(unit) = " + attackSceneManager.allUnits.IndexOf(unit));
            }
        }

        for (int i = 0; i < uiInScene.Count; i++)
        {
            if (PlayerDieIndex.Contains(uiInScene[i].index))
            {
                Debug.Log("uiInScene[i].UI = " + uiInScene[i].UI);
                Debug.Log("uiInScene[i].index = " + uiInScene[i].index);

                RectTransform rUI = uiInScene[i].UI.GetComponent<RectTransform>();
                float height = rUI.rect.height * rUI.localScale.y;
                yield return StartCoroutine(SlideAndFade(uiInScene[i].UI));

                Destroy(uiInScene[i].UI);
                int indexNow = uiInScene.IndexOf(uiInScene[i]);
                uiInScene.Remove(uiInScene[i]);

                for (int j = indexNow; j < uiInScene.Count; j++)
                {
                    StartCoroutine(goUpUI(uiInScene[j].UI, height));
                }
                AddTurnIcon();
                i--;
            }
        }
    }

    private IEnumerator SlideAndFade(GameObject ui)
    {
        RectTransform rectUI = ui.GetComponent<RectTransform>();
        CanvasGroup canvas = ui.GetComponent<CanvasGroup>();

        float duration = 0.3f; // เวลา 0.5 วิ
        float elapsed = 0f;

        Vector2 startPos = rectUI.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(rectUI.rect.width * rectUI.localScale.x, 0);

        float startAlpha = canvas.alpha;
        float targetAlpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // เลื่อนตำแหน่ง
            rectUI.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            // ค่อยๆจาง
            canvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        // ให้ค่าลงล็อคสุดท้าย
        rectUI.anchoredPosition = targetPos;
        canvas.alpha = targetAlpha;
    }

    private IEnumerator goUpUI(GameObject ui, float height)
    {
        RectTransform rectUI = ui.GetComponent<RectTransform>();

        float duration = 0.3f; // เวลาที่ใช้ในการเลื่อน (วินาที)
        float elapsed = 0f;

        Vector2 startPos = rectUI.anchoredPosition;
        Vector2 targetPos = new Vector2(0, rectUI.anchoredPosition.y + height);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            rectUI.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        // ล็อคตำแหน่งสุดท้าย
        rectUI.anchoredPosition = targetPos;
    }
}
