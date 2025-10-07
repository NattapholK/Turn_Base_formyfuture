using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class StatusSystemScript : MonoBehaviour
{
    [Header("เลือดบอส")]
    public int hpEnemy = 16;
    public Slider BossUI;

    [Header("เลือดผู้เล่น")]
    public List<int> hpPlayerList = new List<int>();
    [HideInInspector] public List<int> CurrenthpPlayerList = new List<int>();

    [Header("มานาผู้เล่น")]
    public List<int> manaPlayerList;
    const int maxMana = 100;

    [Header("ตั้งค่าความเร็ว")]
    public List<int> speedPlayerList;
    public int speedBoss = 4;

    private int CurrenthpEnemy;
    private int CabbageIndex;
    private UIManager uiManager;
    private AttackSceneManager attackSceneManager;
    private List<Slider> HPUIList = new List<Slider>();

    void Awake()
    {
        uiManager = GetComponent<UIManager>();
        attackSceneManager = GetComponent<AttackSceneManager>();
        CurrenthpPlayerList = new List<int>(hpPlayerList);
    }
    void Start()
    {
        for (int i = 0; i < manaPlayerList.Count; i++)
        {
            float manaCount = (float)manaPlayerList[i] / maxMana;
            attackSceneManager.pokemonUIList[i].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1 - manaCount;
        }
        CurrenthpEnemy = hpEnemy;
        BossUI.value = (float)CurrenthpEnemy / hpEnemy;
    }

    //ระบบโจมตี จบเกม
    public void playerAttack(int atk)
    {
        CurrenthpEnemy -= atk;
        BossUI.value = (float)CurrenthpEnemy / hpEnemy;
        Debug.Log("hpEnemy = " + hpEnemy);
        Debug.Log("BossUI.value = " + BossUI.value);
        Debug.Log("enemy เหลือ hp " + CurrenthpEnemy);
    }

    public void enemyAttack(int atk, int playerIndex, bool isReduceDmg)
    {
        int attack = atk;
        if (playerIndex < 1 || playerIndex > hpPlayerList.Count)
        {
            Debug.Log("playerIndex ไม่ถูกต้อง");
            return;
        }
        if (isReduceDmg && (CabbageIndex+1) == playerIndex)
        {
            attack = Mathf.RoundToInt(attack * 0.3f);
            Debug.Log("ลดเหลือ dmg" + attack);
        }
        CurrenthpPlayerList[playerIndex - 1] -= attack;

        if (CurrenthpPlayerList[playerIndex - 1] <= 0)
        {
            int gameIndex = 0;
            foreach (BattleUnit unit in attackSceneManager.allUnits)
            {
                if (unit.index == playerIndex - 1)
                {
                    gameIndex = attackSceneManager.allUnits.IndexOf(unit);
                }
            }
            attackSceneManager.allUnits[gameIndex].isdied = true;
            Image uiImage  = attackSceneManager.pokemonProfileUIList[attackSceneManager.allUnits[gameIndex].index].transform.GetChild(0).gameObject.GetComponent<Image>();
            Color c = uiImage.color;
            c.a = 0.4f;
            uiImage.color = c;

            //เกี่ยวกับตายตรงนี้มั้ง
        }

        HPUIList[playerIndex - 1].value = (float)CurrenthpPlayerList[playerIndex - 1] / hpPlayerList[playerIndex - 1];
        Debug.Log("player " + playerIndex + " เหลือ hp " + CurrenthpPlayerList[playerIndex - 1]);
        Debug.Log("hpPlayerList = " + hpPlayerList[playerIndex - 1]);
        Debug.Log("HPUIList[playerIndex - 1].value = " + HPUIList[playerIndex - 1].value);
    }

    public bool checkEndGame()
    {
        if (hpEnemy <= 0 || isAllPlayerDied())
        {
            return true; // เกมจบ
        }
        return false; // เกมยังไม่จบ
    }

    public void endFight()
    {
        if (hpEnemy <= 0)
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
        attackSceneManager.pokemonUIList[playerIndex - 1].transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1 - manaCount;
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
        UI.value = (float)CurrenthpPlayerList[playerIndex] / hpPlayerList[playerIndex];
        HPUIList.Add(UI);
    }
    public void setCabbageIndex(int Index)
    {
        CabbageIndex = Index;
    }
}
