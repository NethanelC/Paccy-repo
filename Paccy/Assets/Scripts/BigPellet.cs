using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPellet : ConsumableBase
{
    public static event Action EatenBigPellet;
    protected override void Consumed()
    {
        ScoreManager.Instance.AddScore(_scoreWorth);
        EatenBigPellet?.Invoke();
        gameObject.SetActive(false);
    }
}
