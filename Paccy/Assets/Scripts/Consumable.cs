using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : MonoBehaviour
{
    [SerializeField] private int _scoreWorth;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.SetActive(false);
        Consumed()?.Invoke();
    }
    protected abstract Action Consumed();
}
