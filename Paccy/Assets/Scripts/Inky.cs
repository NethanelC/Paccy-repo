using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    [SerializeField] private Transform _blinkyTransform;
    protected override Vector2 _eatenTargetTile => new(-0.5f, -0.5f);
    protected override Vector2 _scatterTargetTile => new(13.5f, -17.5f);
    protected override Vector2 ChaseTile()
    {
        Quaternion rotation = _playerTransform.rotation;
        Vector2 relativeBaseTile = _playerTransform.position;
        switch (rotation.eulerAngles.z)
        {
            case 0:
                relativeBaseTile += Vector2.right * 2;
                break;
            case 90:
                relativeBaseTile += new Vector2(-2, 2);
                break;
            case 180:
                relativeBaseTile += Vector2.left * 2;
                break;
            case 270:
                relativeBaseTile += Vector2.down * 2;
                break;
        }
        Vector2 vectorBetween = _blinkyTransform.position - (Vector3)relativeBaseTile;
        return relativeBaseTile - vectorBetween;             
    }
}
