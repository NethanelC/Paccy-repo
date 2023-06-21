using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueGhost : ConsumableBase
{
    private static byte _multiplyBy = 1;
    public void ResetMultiplier()
    {
        _multiplyBy = 1; 
    }
    private void IncreaseMultiplier()
    {
        _multiplyBy *= 2;
    }
    protected override void Consumed()
    {
        print("blueghost");
        SoundManager.Instance.PlaySound(SoundManager.Sound.Ghost);
        ScoreManager.Instance.AddScore(_scoreWorth * _multiplyBy);
        Instantiate(_scorePopup, transform.position, Quaternion.identity).Init(ScorePopup.Type.Ghost, _scoreWorth * _multiplyBy);
        IncreaseMultiplier();
    }
}
