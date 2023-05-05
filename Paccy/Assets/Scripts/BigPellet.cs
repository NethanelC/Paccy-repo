using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPellet : Consumable
{
    public static event Action EatenBigPellet;
    private void Awake()
    {
        GameManager.AllPellets.Add(gameObject);
    }
    protected override Action Consumed()
    {
        if (GameManager.Level < 17 || GameManager.Level == 18)
        {
            return EatenBigPellet;
        }
        else
        {
            return null;
        }  
    }
}
