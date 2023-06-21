using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : ConsumableBase
{
    public static event Action EatenPellet;
    protected override void Consumed()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.Munch);
        ScoreManager.Instance.AddScore(_scoreWorth);
        EatenPellet?.Invoke();
        gameObject.SetActive(false);
    }
}
