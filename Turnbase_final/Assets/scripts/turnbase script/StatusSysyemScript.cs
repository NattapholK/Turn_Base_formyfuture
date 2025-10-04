using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class StatusSysyemScript : MonoBehaviour
{
    [Header("เลือดบอส")]
    public int hpEnemy = 16;

    [Header("เลือดผู้เล่น")]
    public List<int> hpPlayerList;

    [Header("มานาผู้เล่น")]
    public List<int> manaPlayerList;

    [Header("ตั้งค่าความเร็ว")]
    public List<int> speedPlayerList;
    public int speedBoss = 4;

    void Start()
    {

    }


    void Update()
    {

    }

    //ระบบโจมตี จบเกม
    public void playerAttack(int atk)
    {
        hpEnemy -= atk;
        Debug.Log("enemy เหลือ hp " + hpEnemy);
    }

    public void enemyAttack(int atk, int playerIndex)
    {
        if (playerIndex < 1 || playerIndex > hpPlayerList.Count)
        {
            Debug.Log("playerIndex ไม่ถูกต้อง");
            return;
        }
        hpPlayerList[playerIndex - 1] -= atk;
        Debug.Log("player " + playerIndex + " เหลือ hp " + hpPlayerList[playerIndex - 1]);
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
        int manaCost = 0;
        switch (skillID)
        {
            case 1:
                break;
            case 2:
                manaCost = 30;
                break;
            default:
                Debug.Log("สกิลไม่ถูกต้อง");
                break;
        }

        if (playerIndex < 1 || playerIndex > manaPlayerList.Count)
        {
            Debug.Log("playerIndex ไม่ถูกต้อง");
            return;
        }

        if (manaPlayerList[playerIndex - 1] >= manaCost)
        {
            manaPlayerList[playerIndex - 1] -= manaCost;
            Debug.Log("Player " + playerIndex + " เหลือ mana " + manaPlayerList[playerIndex - 1]);
        }
        else
        {
            Debug.Log("Player" + playerIndex + "มานาไม่พอ");
        }
    }

    bool isAllPlayerDied()
    {
        bool allDied = true;
        foreach (int hp in hpPlayerList)
        {
            if (hp > 0)
            {
                allDied = false;
                break;
            }
        }
        return allDied;
    }
}
