using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private Labyrinth.Unit currentUnit;

    public void SetCurrentUnit(Labyrinth.Unit newUnit)
    {
        if (newUnit is not null)
        {
            currentUnit = newUnit;
        }
    }

    public int GetCurrentId()
    {
        return currentUnit.Info.Id;
    }
}
