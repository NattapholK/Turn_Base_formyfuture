using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Unit
{
    public GameObject UI;
    public int index;
}

public class UI : MonoBehaviour
{
    public float space = 0f;
    public GameObject backgroundUI;
    public GameObject parentPanel;
    private Transform parentPanelTranform;

    private float firstX;
    private int dieCount;
    private int LastTurn;
    private float LastOffset;
    [HideInInspector] public List<Character> characters;
    private List<Unit> uiInScene = new List<Unit>();
    private float bgUIHight;
    private RectTransform bgUIRecttranform;

    void Awake()
    {
        parentPanelTranform = parentPanel.GetComponent<Transform>();
        RectTransform rfx = parentPanel.GetComponent<RectTransform>();
        firstX = rfx.rect.width * rfx.localScale.y / 2;
        bgUIRecttranform = backgroundUI.GetComponent<RectTransform>();
        bgUIHight = bgUIRecttranform.rect.height;
    }

    public void CreateTurnIcons()
    {
        Debug.Log("เริ่มสร้าง");
        float offsetY = 0f;

        Debug.Log("_Manager.characters.Count = " + characters.Count);
        for (int i = 0; offsetY < bgUIHight; i++)
        {
            if (i >= characters.Count)
            {
                i = 0;
            }
            LastTurn = i + 1;

            GameObject icon = Instantiate(characters[i]._TurnUI, parentPanelTranform);
            RectTransform rt = icon.GetComponent<RectTransform>();

            float height = rt.rect.height * rt.localScale.y + space;

            // จัดเรียงแบบ top-down
            rt.anchoredPosition = new Vector2(-firstX, -offsetY);

            LastOffset = offsetY;
            offsetY += height;

            uiInScene.Add(new Unit
            {
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

        foreach (Unit UI in uiInScene)
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
        do
        {
            LastTurn++;
            if (LastTurn >= characters.Count)
            {
                LastTurn = 0;
            }
        }
        while(characters[LastTurn].GetHp() <= 0);

        GameObject icon = Instantiate(characters[LastTurn]._TurnUI, parentPanelTranform);
        RectTransform rt = icon.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-firstX, -LastOffset);

        uiInScene.Add(new Unit
        {
            UI = icon,
            index = LastTurn
        });
    }

    public IEnumerator ScaleUI(RectTransform UI, string scaleType)
    {
        float duration = 0.2f;
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

        foreach (Character c in characters)
        {
            if (c.GetHp() <= 0)
            {
                PlayerDieIndex.Add(characters.IndexOf(c));
                Debug.Log("characters.IndexOf(c) = " + characters.IndexOf(c));
            }
        }

        Debug.Log("############################");
        Debug.Log("PlayerDieIndex.Count = " + PlayerDieIndex.Count);
        Debug.Log("dieCount = " + dieCount);
        

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
