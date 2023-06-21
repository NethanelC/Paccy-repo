using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConsumableBase : MonoBehaviour
{
    [SerializeField] protected ScorePopup _scorePopup;
    [SerializeField] protected ushort _scoreWorth;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled)
        {
            return;
        }
        Consumed();
    }
    protected abstract void Consumed();
}
