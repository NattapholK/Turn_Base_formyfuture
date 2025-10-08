using System.Collections.Generic;
using NUnit.Framework;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class StatusSystemScript : MonoBehaviour
{
    [Header("Other Setting")]
    public Slider BossUI;
    public GameObject FloatingTextPrefab;
    public GameObject DamageUI;


    [HideInInspector] public List<int> CurrenthpPlayerList = new List<int>();
    [HideInInspector] public List<int> manaPlayerList;
    const int maxMana = 100;
    private int CurrenthpEnemy;
    private int CabbageIndex;
    private TextMeshProUGUI DmgText;
    private UIManager uiManager;
    private AttackSceneManager attackSceneManager;
    private List<Slider> HPUIList = new List<Slider>();

    void Awake()
    {
        uiManager = GetComponent<UIManager>();
        attackSceneManager = GetComponent<AttackSceneManager>();

        CurrenthpEnemy = attackSceneManager.hpEnemy;
        BossUI.value = (float)CurrenthpEnemy / attackSceneManager.hpEnemy;

        for (int i = 0; i < attackSceneManager.playerData.Count; i++)
        {
            manaPlayerList.Add(0);
            float manaCount = (float)manaPlayerList[i] / maxMana;
            CurrenthpPlayerList.Add(attackSceneManager.playerData[i].hpPlayer);
            setattack setPlayer = attackSceneManager.playerData[i].playerObject.GetComponent<setattack>();
            setPlayer.isPlayer = true;
            setPlayer.NamePlayer = "player" + (i + 1).ToString();
            setPlayer.attackManager = gameObject;
            setPlayer.playerAtk = attackSceneManager.playerData[i].atkPlayer;
            attackSceneManager.playerData[i].playerUI.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1 - manaCount;
        }
        attackSceneManager.enemy.GetComponent<setattack>().isPlayer = false;
        attackSceneManager.enemy.GetComponent<setattack>().bossAtk = attackSceneManager.atkBoss;
        attackSceneManager.enemy.GetComponent<setattack>().attackManager = gameObject;
    }

    void Start()
    {
        DmgText = DamageUI.transform.GetChild(DamageUI.transform.childCount - 1).gameObject.GetComponent<TextMeshProUGUI>();
    }

    //ระบบโจมตี จบเกม
    public void playerAttack(int atk)
    {
        CurrenthpEnemy -= atk;
        if (FloatingTextPrefab != null)
        {
            ShowFloatingText(attackSceneManager.enemy, atk, 4.2f);
        }

        if (CurrenthpEnemy <= 0)
        {
            int gameIndex = 0;
            foreach (BattleUnit unit in attackSceneManager.allUnits)
            {
                if (unit.isBoss)
                {
                    gameIndex = attackSceneManager.allUnits.IndexOf(unit);
                    Debug.Log("unit = " + attackSceneManager.allUnits.IndexOf(unit));
                    break;
                }
            }
            attackSceneManager.allUnits[gameIndex].isdied = true;
            Debug.Log("ทำงาน");

        }

        BossUI.value = (float)CurrenthpEnemy / attackSceneManager.hpEnemy;
        StartCoroutine(setDmgTextUI(atk));
        Debug.Log("hpEnemy = " + attackSceneManager.hpEnemy);
        Debug.Log("BossUI.value = " + BossUI.value);
        Debug.Log("enemy เหลือ hp " + CurrenthpEnemy);
    }

    public void enemyAttack(int atk, int playerIndex, bool isReduceDmg)
    {
        int attack = atk;
        if (playerIndex < 1 || playerIndex > attackSceneManager.playerData.Count)
        {
            Debug.Log("playerIndex ไม่ถูกต้อง");
            return;
        }
        if (isReduceDmg && (CabbageIndex + 1) == playerIndex)
        {
            attack = Mathf.RoundToInt(attack * 0.3f);
            Debug.Log("ลดเหลือ dmg" + attack);
        }
        CurrenthpPlayerList[playerIndex - 1] -= attack;
        if (FloatingTextPrefab != null)
        {
            ShowFloatingText(attackSceneManager.playerData[playerIndex - 1].playerObject, atk, 1f);
        }

        if (CurrenthpPlayerList[playerIndex - 1] <= 0)
        {
            int gameIndex = 0;
            foreach (BattleUnit unit in attackSceneManager.allUnits)
            {
                if (unit.index == playerIndex - 1)
                {
                    gameIndex = attackSceneManager.allUnits.IndexOf(unit);
                    break;
                }
            }
            attackSceneManager.allUnits[gameIndex].isdied = true;
            Image uiImage = attackSceneManager.playerData[attackSceneManager.allUnits[gameIndex].index].playerProfileUI.transform.GetChild(0).gameObject.GetComponent<Image>();
            Color c = uiImage.color;
            c.a = 0.4f;
            uiImage.color = c;

            //เกี่ยวกับตายตรงนี้มั้ง
        }

        HPUIList[playerIndex - 1].value = (float)CurrenthpPlayerList[playerIndex - 1] / attackSceneManager.playerData[playerIndex - 1].hpPlayer;
    }

    public bool checkEndGame()
    {
        if (CurrenthpEnemy <= 0 || isAllPlayerDied())
        {
            return true; // เกมจบ
        }
        return false; // เกมยังไม่จบ
    }

    public void endFight()
    {
        if (CurrenthpEnemy <= 0)
        {
            Debug.Log("Enemy defeated!");
        }
        else if (isAllPlayerDied())
        {
            Debug.Log("Player defeated!");
        }
    }

    public void useMana(int playerIndex, int skillID)
    {
        if (playerIndex < 1 || playerIndex > manaPlayerList.Count)
        {
            Debug.Log("playerIndex ไม่ถูกต้อง");
            return;
        }

        int manaCost = 0;

        switch (skillID)
        {
            case 1:
                manaCost = maxMana / 2;
                if (manaPlayerList[playerIndex - 1] != maxMana)
                {
                    manaPlayerList[playerIndex - 1] += manaCost;
                }
                Debug.Log("Player " + playerIndex + " เหลือ mana " + manaPlayerList[playerIndex - 1]);
                break;
            case 2:
                manaCost = maxMana;
                manaPlayerList[playerIndex - 1] -= manaCost;
                Debug.Log("Player " + playerIndex + " เหลือ mana " + manaPlayerList[playerIndex - 1]);
                break;
            default:
                Debug.Log("สกิลไม่ถูกต้อง");
                break;
        }
        float manaCount = (float)manaPlayerList[playerIndex - 1] / maxMana;
        attackSceneManager.playerData[playerIndex - 1].playerUI.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1 - manaCount;
    }

    bool isAllPlayerDied()
    {
        bool allDied = true;
        foreach (int hp in CurrenthpPlayerList)
        {
            if (hp > 0)
            {
                allDied = false;
                break;
            }
        }
        return allDied;
    }

    public int maxManaData()
    {
        return maxMana;
    }

    public void AddHpUI(GameObject hpUI, int playerIndex)
    {
        Slider UI = hpUI.GetComponent<Slider>();
        UI.value = (float)CurrenthpPlayerList[playerIndex] / attackSceneManager.playerData[playerIndex].hpPlayer;
        HPUIList.Add(UI);
    }
    public void setCabbageIndex(int Index)
    {
        CabbageIndex = Index;
    }

    private IEnumerator setDmgTextUI(int atk)
    {
        // DamageUI
        DmgText.text = atk.ToString();
        DamageUI.SetActive(true);
        CanvasGroup canvasUI = DamageUI.GetComponent<CanvasGroup>();
        RectTransform rectUI = DamageUI.GetComponent<RectTransform>();
        Vector2 startPos = rectUI.anchoredPosition;
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(uiManager.SlideAndFade(DamageUI));
        rectUI.anchoredPosition = startPos;
        canvasUI.alpha = 1f;
        DamageUI.SetActive(false);
    }

    void ShowFloatingText(GameObject player, int dmg, float radius)
    {
        Debug.Log("ShowFloatingText ทำงานแล้ว");

        Vector3 randomOffset = new Vector3(
            Random.Range(-radius, radius),
            Random.Range(0.5f, 1.5f), // ยกขึ้นจากพื้นเล็กน้อย
            -radius
        );

        Vector3 spawnPos = player.transform.position + randomOffset;

        // Instantiate
        var go = Instantiate(FloatingTextPrefab, spawnPos, Quaternion.identity, transform);
        var tmp = go.GetComponent<TextMeshPro>();
        tmp.text = dmg.ToString();
        Destroy(go, 2f);
    }
}
