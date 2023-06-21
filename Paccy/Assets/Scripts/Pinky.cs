using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : Ghost
{
    protected override Vector2 _eatenTargetTile => new(-0.5f, -0.5f);
    protected override Vector2 _scatterTargetTile => new(-11.5f, 16.5f);
    protected override Vector2 ChaseTile()
    {
        Quaternion rotation = _playerTransform.rotation;
        Vector2 relativeTile = _playerTransform.position;
        switch(rotation.eulerAngles.z)
        {
            case 0:
                relativeTile += Vector2.right * 4;
                break;
            case 90:
                relativeTile += new Vector2(-4, 4);
                break;
            case 180:
                relativeTile += Vector2.left * 4;
                break;
            case 270:
                relativeTile += Vector2.down * 4;
                break;
        }
        return relativeTile;
    }
}
