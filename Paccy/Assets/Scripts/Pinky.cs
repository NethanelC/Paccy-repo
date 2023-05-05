using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : Ghost
{
    protected override Vector2 ChaseTile()
    {
        Quaternion rotation = _playerTransform.rotation;
        Vector2 relativeTile = new(_playerTransform.position.x, _playerTransform.position.y);
        switch(rotation.eulerAngles.z)
        {
            case 0:
                relativeTile += new Vector2(4, 0);
                break;
            case 90:
                relativeTile += new Vector2(-4, 4);
                break;
            case 180:
                relativeTile += new Vector2(-4, 0);
                break;
            case 270:
                relativeTile += new Vector2(0, -4);
                break;
        }
        return relativeTile;
    }
}
