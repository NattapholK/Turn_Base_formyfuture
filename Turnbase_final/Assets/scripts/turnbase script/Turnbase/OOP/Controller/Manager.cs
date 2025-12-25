using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public Player[] players;
    public Enemy[] enemies;
    private List<Character> characters = new List<Character>();
    private int Current_Index = 0;

    void Awake()
    {
        characters.AddRange(players.Where(p => p != null));
        characters.AddRange(enemies.Where(e => e != null));
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
    }

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        Debug.Log("StartTurn ทำงาน");
        characters[Current_Index].MyTurn = true;
    }

    public void EndTurn()
    {
        Debug.Log("EndTurn ทำงาน");
        characters[Current_Index].MyTurn = false;

        do
        {
            Current_Index++;
            if (Current_Index >= characters.Count) Current_Index = 0; // วนกลับรอบใหม่
        }
        while (characters[Current_Index].GetHp() <= 0);

        StartTurn();
    }

    public void InsertionSort(int Index)
    {
        Debug.Log("InsertionSort ทำงาน");
        Character temp = characters[Index];
        int j = Index - 1;
        while (j >= 0 && (characters[j].GetSpeed() > temp.GetSpeed()))
        {
            characters[j+1] = characters[j];
            j--;
        }
        characters[j+1] = temp;
    }
}
