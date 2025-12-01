using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    public float space = 0f;
    public GameObject backgroundUI;
    public GameObject parentPanel;
    private Transform parentPanelTranform;

    private float firstX;
    private int dieCount;
    private int LastTurn;
    private float LastOffset;
    private List<UIUnit> uiUnits = new List<UIUnit>();
    private List<UIUnit> uiInScene = new List<UIUnit>();
    private float bgUIHight;
    private RectTransform bgUIRecttranform;
    private AttackSceneManager attackSceneManager;
    void Awake()
    {
        attackSceneManager = GetComponent<AttackSceneManager>();
        parentPanelTranform = parentPanel.GetComponent<Transform>();
        RectTransform rfx = parentPanel.GetComponent<RectTransform>();
        firstX = rfx.rect.width * rfx.localScale.y / 2;
    }
    void Start()
    {

        for (int i = 0; i < attackSceneManager.playerData.Count; i++)
        {
            uiUnits.Add(new UIUnit
            {
                speed = attackSceneManager.playerData[i].speedPlayer,//statusScript.speedPlayerList[i],
                UI = attackSceneManager.playerData[i].turnPlayerUIPrefab,
                index = i
            });
        }
        uiUnits.Add(new UIUnit
        {
            speed = attackSceneManager.speedBoss,//statusScript.speedBoss,
            UI = attackSceneManager.turnEnemyUIPrefab,
            index = uiUnits.Count
        });

        // uiUnits = uiUnits.OrderByDescending(u => u.speed).ToList();
        for(int i = 0; i < uiUnits.Count ; i++)
        {
            Sort(uiUnits, i);
        }

        bgUIRecttranform = backgroundUI.GetComponent<RectTransform>();
        bgUIHight = bgUIRecttranform.rect.height;
        CreateTurnIcons();
    }

    void Sort(List<UIUnit> list, int Index)
    {
        int j = Index - 1;
        UIUnit temp = list[Index];
        while(j >= 0 && list[j].speed > temp.speed)
        {
            list[j + 1] = list[j];
            j--;
        }
        list[j + 1] = temp;
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

            GameObject icon = Instantiate(uiUnits[i].UI, parentPanelTranform);
            RectTransform rt = icon.GetComponent<RectTransform>();

            float height = rt.rect.height * rt.localScale.y + space;

            // จัดเรียงแบบ top-down
            rt.anchoredPosition = new Vector2(-firstX, -offsetY);

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
        if (uiInScene.Count > 0)
        {
            StartCoroutine(firstUIUpScale());
        }
    }
    private IEnumerator DeleteTurnIcon()
    {
        RectTransform firstUI = uiInScene[0].UI.GetComponent<RectTransform>();
        float height = firstUI.rect.height * firstUI.localScale.y;

        Destroy(uiInScene[0].UI);
        uiInScene.RemoveAt(0);

        foreach (UIUnit UI in uiInScene)
        {
            goUpUI(UI.UI, height + space);
        }

        if (uiInScene.Count > 0)
        {
            yield return StartCoroutine(firstUIUpScale());
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
        GameObject icon = Instantiate(uiUnits[LastTurn].UI, parentPanelTranform);
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-firstX, -LastOffset);

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
        float duration = 0.05f;
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
        Debug.Log("ตรงนี้CheckDiedPlayerIconทำงาน");
        List<int> PlayerDieIndex = new List<int>();

        foreach (BattleUnit unit in attackSceneManager.allUnits)
        {
            if (unit.isdied)
            {
                PlayerDieIndex.Add(attackSceneManager.allUnits.IndexOf(unit));
                Debug.Log("attackSceneManager.allUnits.IndexOf(unit) = " + attackSceneManager.allUnits.IndexOf(unit));
                if (unit.isBoss)
                {
                    Debug.Log("บอสก็ตาย");
                }
            }
            if (unit.isBoss)
            {
                Debug.Log("มีเช็คบอส");
            }
        }

        if (dieCount != PlayerDieIndex.Count)
        {
            yield return StartCoroutine(firstUIDownScale());
        }
        else
        {
            Debug.Log("asdasdasdas ทำงาน");
            yield return StartCoroutine(firstUIDownScale());
            StartCoroutine(DeleteTurnIcon());
            yield break;
        }

        yield return StartCoroutine(DeleteAndCreateDieTurn(PlayerDieIndex));

        if (uiInScene.Count > 0)
        {
            StartCoroutine(DeleteTurnIcon());
        }

        dieCount = PlayerDieIndex.Count;
        Debug.Log("dieCount = " + dieCount);
    }

    private IEnumerator DeleteAndCreateDieTurn(List<int> listdie)
    {
        for (int i = 0; i < uiInScene.Count; i++)
        {
            if (listdie.Contains(uiInScene[i].index))
            {
                Debug.Log("uiInScene[i].UI = " + uiInScene[i].UI);
                Debug.Log("uiInScene[i].index = " + uiInScene[i].index);

                RectTransform rUI = uiInScene[i].UI.GetComponent<RectTransform>();
                float height = rUI.rect.height * rUI.localScale.y;
                yield return StartCoroutine(SlideAndFade(uiInScene[i].UI));

                Destroy(uiInScene[i].UI);
                int indexNow = uiInScene.IndexOf(uiInScene[i]);
                uiInScene.Remove(uiInScene[i]);

                Debug.Log("uiInScene.Count = " + uiInScene.Count);
                for (int j = indexNow; j < uiInScene.Count; j++)
                {
                    Debug.Log("j = " + j);
                    Debug.Log("uiInScene[j].UI = " + uiInScene[j].UI);
                    goUpUI(uiInScene[j].UI, height + space);
                }
                Debug.Log("Add ละ " + i);
                AddTurnIcon();
                i--;
            }
        }
        
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator SlideAndFade(GameObject ui)
    {
        RectTransform rectUI = ui.GetComponent<RectTransform>();
        CanvasGroup canvas = ui.GetComponent<CanvasGroup>();

        float duration = 0.4f; // เวลา 0.4 วิ
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

    private void goUpUI(GameObject ui, float height)
    {
        RectTransform rectUI = ui.GetComponent<RectTransform>();

        Vector2 targetPos = new Vector2(-firstX, rectUI.anchoredPosition.y + height);

        rectUI.anchoredPosition = targetPos;
    }

    private IEnumerator firstUIUpScale()
    {
        uiInScene[0].UI.transform.GetChild(0).gameObject.SetActive(false);
        uiInScene[0].UI.transform.GetChild(1).gameObject.SetActive(true);
        RectTransform ui = uiInScene[0].UI.GetComponent<RectTransform>();

        Vector3 startScale = ui.localScale;
        Vector3 targetScale = startScale * 1.2f;

        Vector2 startPos = ui.anchoredPosition;
        float addedHeight = ui.rect.height * (targetScale.y - startScale.y);
        Vector2 targetPos = startPos + new Vector2(0, addedHeight / 2f);

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            ui.localScale = Vector3.Lerp(startScale, targetScale, t);
            ui.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        ui.localScale = targetScale;
        ui.anchoredPosition = targetPos;
    }

    
    private IEnumerator firstUIDownScale()
    {
        RectTransform ui = uiInScene[0].UI.GetComponent<RectTransform>();

        Vector3 startScale = ui.localScale;
        Vector3 targetScale = startScale * (1f / 1.2f);

        Vector2 startPos = ui.anchoredPosition;
        float addedHeight = ui.rect.height * (targetScale.y - startScale.y);
        float addedWidth = ui.rect.width * (targetScale.x - startScale.x);
        Vector2 targetPos = startPos + new Vector2(addedWidth / 2f, addedHeight / 2f);

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            ui.localScale = Vector3.Lerp(startScale, targetScale, t);
            ui.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        ui.localScale = targetScale;
        ui.anchoredPosition = targetPos;
    }


}
