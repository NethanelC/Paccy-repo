using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : Consumable
{
    public static event Action EatenPellet;
    private void Awake()
    {
        GameManager.AllPellets.Add(gameObject);
    }
    protected override Action Consumed()
    {
        return EatenPellet;
    }
}
