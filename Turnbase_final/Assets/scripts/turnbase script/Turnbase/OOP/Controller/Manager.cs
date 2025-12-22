using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public Character[] characters;
    private List<Character> c;
    private int Current_Index = 0;

    void Awake()
    {
        int i = 0;
        foreach (Character unit in characters)
        {
            c.Add(unit);
            InsertionSort(i);
            i++;
        }
    }

    void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        c[Current_Index].MyTurn = true;
    }

    public void EndTurn()
    {
        c[Current_Index].MyTurn = false;

        do
        {
            Current_Index++;
            if (Current_Index >= c.Count) Current_Index = 0; // วนกลับรอบใหม่
        }
        while (c[Current_Index].GetHp() <= 0);

        StartTurn();
    }

    public void InsertionSort(int Index)
    {
        Character temp = c[Index];
        int j = Index - 1;
        while (j >= 0 && (c[j].GetSpeed() > temp.GetSpeed()))
        {
            c[j+1] = c[j];
            j--;
        }
        c[j+1] = temp;
    }
}
