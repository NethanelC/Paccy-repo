using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : Ghost
{
    protected override Vector2 ChaseTile()
    {
        if (Vector2.Distance(_playerTransform.position, transform.position) <= 8)
        {
            return ScatterTile();
        }
        else
        {
            return _playerTransform.position;
        }   
    }
}