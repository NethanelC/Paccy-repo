using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Symbol : Consumable
{
    public static event Action EatenSymbol;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        Destroy(gameObject, UnityEngine.Random.Range(9, 10));
    }
    protected override Action Consumed()
    {
        return EatenSymbol;
    }
}
