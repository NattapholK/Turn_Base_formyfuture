using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

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
    public void DeleteTurnIcon()
    {
        RectTransform firstUI = uiInScene[0].UI.GetComponent<RectTransform>();
        float height = firstUI.rect.height * firstUI.localScale.y;

        Destroy(uiInScene[0].UI);
        uiInScene.Remove(uiInScene[0]);

        foreach (UIUnit UI in uiInScene)
        {
            RectTransform rectUI = UI.UI.GetComponent<RectTransform>();
            rectUI.anchoredPosition = new Vector2(0, rectUI.anchoredPosition.y + height);
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

    public void CheckDiedPlayerIcon(int index)
    {
        int dieIndex = -1;
        foreach (BattleUnit unit in attackSceneManager.allUnits)
        {
            if (unit.index == index)
            {
                dieIndex = attackSceneManager.allUnits.IndexOf(unit);
                break;
            }
        }

        if (dieIndex < 0) return;

        if (attackSceneManager.allUnits[dieIndex].isdied)
        {
            for (int i=0; i < uiInScene.Count; i++)
            {
                if (uiInScene[i].index == dieIndex)
                {
                    RectTransform rUI = uiInScene[i].UI.GetComponent<RectTransform>();
                    float height = rUI.rect.height * rUI.localScale.y;

                    Destroy(uiInScene[i].UI);
                    int indexNow = uiInScene.IndexOf(uiInScene[i]);
                    uiInScene.Remove(uiInScene[i]);

                    for (int j=indexNow; j < uiInScene.Count; j++)
                    {
                        RectTransform rectUI = uiInScene[j].UI.GetComponent<RectTransform>();
                        rectUI.anchoredPosition = new Vector2(0, rectUI.anchoredPosition.y + height);
                    }
                    AddTurnIcon();
                }
            }
        }
        else
        {
            return;
        }


    }
}
