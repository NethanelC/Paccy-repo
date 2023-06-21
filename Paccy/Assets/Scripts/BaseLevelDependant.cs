using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLevelDependant : MonoBehaviour
{
    private Vector2 _startPosition;
    public abstract void InitByLevel(byte level);
    public abstract void NewRound();
    protected virtual void Awake()
    {
        _startPosition = transform.position;
    }
    public virtual void BasePositionAndFacingSide()
    {
        gameObject.SetActive(true);
        transform.position = _startPosition;
    }
}
