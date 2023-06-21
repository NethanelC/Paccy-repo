using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : ConsumableBase
{
    public static event Action EatenSymbol;
    private void Awake()
    {
        Destroy(gameObject, UnityEngine.Random.Range(9, 10));
    }
    protected override void Consumed()
    {
        ScoreManager.Instance.AddScore(_scoreWorth);
        EatenSymbol?.Invoke();
        gameObject.SetActive(false);
    }
}
