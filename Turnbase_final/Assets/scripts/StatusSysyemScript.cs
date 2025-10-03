using UnityEngine;

public class StatusSysyemScript : MonoBehaviour
{
    [Header("เลือดบอส")]
    public int hpEnemy = 16;

    [Header("เลือดผู้เล่น")]
    public int hpPlayer1 = 20;
    public int hpPlayer2 = 20;
    public int hpPlayer3 = 20;

    [Header("มานาผู้เล่น")]
    public int mana1 = 100;
    public int mana2 = 100;
    public int mana3 = 100;
    
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

    public void enemyAttack(int atk, string target)
    {
        switch (target)
        {
            case "player1":
                hpPlayer1 -= atk;
                Debug.Log("player1 เหลือ hp " + hpPlayer1);
                break;
            case "player2":
                hpPlayer2 -= atk;
                Debug.Log("player2 เหลือ hp " + hpPlayer2);
                break;
            case "player3":
                hpPlayer3 -= atk;
                Debug.Log("player3 เหลือ hp " + hpPlayer3);
                break;
            default:
                Debug.Log("เป้าหมายไม่ถูกต้อง");
                break;
        }
    }

    public bool checkEndGame()
    {
        if (hpEnemy <= 0 || (hpPlayer1 <= 0 && hpPlayer2 <= 0 && hpPlayer3 <= 0))
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
        else if (hpPlayer1 <= 0 && hpPlayer2 <= 0 && hpPlayer3 <= 0)
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
                // สกิลธรรมดา ไม่เสียมานา
                break;
            case 2:
                manaCost = 30;
                break;
            default:
                Debug.Log("สกิลไม่ถูกต้อง");
                break;
        }
        switch (playerIndex)
        {
            case 1:
                if (mana1 < manaCost)
                {
                    Debug.Log("Player 1 มานาไม่พอ");
                    break;
                }
                mana1 -= manaCost;
                Debug.Log("Player 1 เหลือ mana " + mana1);
                break;
            case 2:
                if (mana2 < manaCost)
                {
                    Debug.Log("Player 2 มานาไม่พอ");
                    break;
                }
                mana2 -= manaCost;
                Debug.Log("Player 2 เหลือ mana " + mana2);
                break;
            case 3:
                if (mana3 < manaCost)
                {
                    Debug.Log("Player 3 มานาไม่พอ");
                    break;
                }
                mana3 -= manaCost;
                Debug.Log("Player 3 เหลือ mana " + mana3);
                break;
            default:
                Debug.Log("ผู้เล่นไม่ถูกต้อง");
                break;
        }
    }
}
