using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    [Header("Character")]
    public Player[] players;
    public Enemy[] enemies;

    [Header("Camera Setting")]
    public GameObject _cameraObj;
    public float moveTime = 1.5f; // เวลาในการเคลื่อน

    [Header("Game Setting")]
    public int SceneIndex = 0;
    public GameObject _FloatingTextPrefab;
    public GameObject _UICanvas;
    public Transform _EndPosition;
    public GameObject _DamageUI;

    [Header("Scene Manage")]
    public string NextSceneName;
    public string LostSceneName;
    public ManagerValue managerValue;
    public int GameSceneIndex = 0;//สร้างไว้ไม่กล้าแตะอันก่อนหน้า

    private List<Character> characters = new List<Character>();
    private Coroutine moveCoroutine;
    private UI ui;
    private int Current_Index = 0;
    private TextMeshProUGUI DmgText;
    private Vector2 startDamgeUIPos;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ui = GetComponent<UI>();
        characters.AddRange(players.Where(p => p != null));
        characters.AddRange(enemies.Where(e => e != null));
        for(int i = 0; i< characters.Count; i++)
        {
            Debug.Log("i = " + i + ", character = " + characters[i]);
        }
    }

    void Start()
    {
        for(int i = 0; i< characters.Count; i++)
        {
            if(characters[i] is Player p)
            {
                p.enemies = enemies;
            }
            else if(characters[i] is Enemy e)
            {
                e.players = players;
            }
            InsertionSort(i);
        }
        for(int i = 0; i< characters.Count; i++)
        {
            Debug.Log("i = " + i + ", character = " + characters[i]);
        }
        RectTransform rectUI = _DamageUI.GetComponent<RectTransform>();
        startDamgeUIPos = rectUI.anchoredPosition;
        DmgText = _DamageUI.transform.GetChild(_DamageUI.transform.childCount - 1).gameObject.GetComponent<TextMeshProUGUI>();
        DmgText.text = "0";
        ui.characters.AddRange(characters);
        ui.CreateTurnIcons();
        CameraManager.Instance.CameraChangeTurnTransition(Current_Index);
        StartTurn();
    }

    public void StartTurn()
    {
        Debug.Log("StartTurn ทำงาน");
        Character c = characters[Current_Index];
        if(c is Player p)
        {
            StartCoroutine(ui.ScaleUI(p._ProfileUIObject.GetComponent<RectTransform>(), "up"));
            for(int i = 0; i < players.Count(); i++)
            {
                if(players[i] == p)
                {
                    CameraManager.Instance.CameraChangeTurnTransition(i);
                    break;
                }
            }
        }
        else if(c is Enemy e)
        {
            MoveToPosition(c._CameraPos.position);
        }
        c.MyTurn = true;
    }

    public void EndTurn()
    {
        Debug.Log("EndTurn ทำงาน");
        Character c = characters[Current_Index];
        c.MyTurn = false;

        StartCoroutine(ui.CheckDiedPlayerIcon());

        if(c is Player p)
        {
            StartCoroutine(ui.ScaleUI(p._ProfileUIObject.GetComponent<RectTransform>(), "down"));
        }

        if (isAllPlayerDied())
        {
            Debug.Log("จบแล้ว");
            MoveToPosition(_EndPosition.position);
            // endFight();
        }
        else
        {
            do
            {
                Current_Index++;
                if (Current_Index >= characters.Count) Current_Index = 0; // วนกลับรอบใหม่
            }
            while (characters[Current_Index].Current_Hp <= 0);

            StartTurn();
        }
    }

    public void InsertionSort(int Index)
    {
        Debug.Log("InsertionSort ทำงาน");
        Character temp = characters[Index];
        int j = Index - 1;
        while (j >= 0 && (characters[j].Current_Speed < temp.Current_Speed))
        {
            characters[j+1] = characters[j];
            j--;
        }
        characters[j+1] = temp;
    }


    // เรียกใช้เพื่อย้ายกล้องไปตำแหน่งใหม่
    public void MoveToPosition(Vector3 newPos)
    {
        if (_cameraObj == null)
        {
            Debug.LogWarning("ยังไม่ได้กำหนด cameraObj ใน Inspector");
            return;
        }

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(SmoothMove(newPos));
    }

    IEnumerator SmoothMove(Vector3 targetPos)
    {
        GameObject cameraObj = CameraManager.Instance.CodeMovementObj; //อยู่ในช่วงลองผิดลองถูก

        Vector3 startPos = cameraObj.transform.position;
        float elapsed = 0f;


        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveTime);

            cameraObj.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        cameraObj.transform.position = targetPos;
    }

    public IEnumerator setDmgTextUI(int atk)
    {
        CanvasGroup canvasUI = _DamageUI.GetComponent<CanvasGroup>();
        RectTransform rectUI = _DamageUI.GetComponent<RectTransform>();

        // ถ้ามี coroutine นี้รันอยู่แล้ว ให้หยุดก่อน
        StopCoroutine(nameof(setDmgTextUI));

        _DamageUI.SetActive(true);
        canvasUI.alpha = 1f;
        rectUI.anchoredPosition = startDamgeUIPos;

        int oldDmg = 0;
        int.TryParse(DmgText.text, out oldDmg);
        DmgText.text = (oldDmg + atk).ToString();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ui.SlideAndFade(_DamageUI));

        // reset state
        rectUI.anchoredPosition = startDamgeUIPos;
        DmgText.text = "0";
        canvasUI.alpha = 1f;
        _DamageUI.SetActive(false);
    }

    private bool isAllPlayerDied()
    {
        bool state = false;
        foreach(Character c in characters)
        {
            if(c.Current_Hp <= 0 && c is Player p)
            {
                state = true;
            }
        }

        return state;
    }

    // private bool isAllEmenyDied()
    // {
    //     bool state = false;
    //     foreach(Character c in characters)
    //     {
    //         if(c.GetHp() <= 0 && c is Enemy e)
    //         {
    //             state = true;
    //         }
    //     }

    //     return state;
    // }

    // private void endFight()
    // {
    //     float timer = 0;
    //     while(timer < 2f)
    //     {
    //         timer += Time.deltaTime;
    //     }
    //     if (isAllEmenyDied())
    //     {
    //         switch (GameSceneIndex)
    //         {
    //             case -1:
    //                 break;
    //             case 0:
    //                 managerValue.isCompleteGame1 = true;
    //                 break;
    //             case 1:
    //                 managerValue.isCompleteGame2 = true;
    //                 break;
    //             case 2:
    //                 managerValue.isCompleteGame3 = true;
    //                 break;
    //         }
    //         Debug.Log("Enemy defeated!");
    //         if(!string.IsNullOrEmpty(NextSceneName))
    //         {
    //             Debug.Log("LevelManager.Instance.LoadScene(NextSceneName); ทำงาน");
    //             LevelManager.Instance.LoadScene(NextSceneName);
    //         }
    //     }
    //     else if (isAllPlayerDied())
    //     {
    //         Debug.Log("Player defeated!");
    //         if(!string.IsNullOrEmpty(LostSceneName))
    //         {
    //             Debug.Log("LevelManager.Instance.LoadScene(LostSceneName); ทำงาน");
    //             LevelManager.Instance.LoadScene(LostSceneName);
    //         }
    //     }
    // }
}
