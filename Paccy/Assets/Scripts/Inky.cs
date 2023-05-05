using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    [SerializeField] private Transform _blinkyTransform;
    protected override Vector2 ChaseTile()
    {
        Quaternion rotation = _playerTransform.rotation;
        Vector2 relativeBaseTile = new(_playerTransform.position.x, _playerTransform.position.y);
        switch (rotation.eulerAngles.z)
        {
            case 0:
                relativeBaseTile += new Vector2(2, 0);
                break;
            case 90:
                relativeBaseTile += new Vector2(-2, 2);
                break;
            case 180:
                relativeBaseTile += new Vector2(-2, 0);
                break;
            case 270:
                relativeBaseTile += new Vector2(0, 2);
                break;
        }
        Vector2 vectorBetween = new Vector2(_blinkyTransform.position.x, _blinkyTransform.position.y) - relativeBaseTile;
        return relativeBaseTile - vectorBetween;             
    }
}
