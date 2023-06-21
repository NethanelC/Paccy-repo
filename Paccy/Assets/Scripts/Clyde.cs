using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : Ghost
{
    protected override Vector2 _eatenTargetTile => new(-0.5f, -0.5f);
    protected override Vector2 _scatterTargetTile => new (-13.5f, -17.5f);
    protected override Vector2 ChaseTile()
    {
        return Vector2.Distance(_playerTransform.position, transform.position) <= 8 ? _scatterTargetTile : _playerTransform.position;
    }
}