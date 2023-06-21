using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Ghost
{
    [SerializeField] private Sprite[] _furiousEyesSprites = new Sprite[4];
    private readonly float[] _currentLevelElroySpeed = { 0.126f, 0.112f, 0.112f, 0.112f, 0.1f };
    private float _elroySpeed;
    private bool _elroyEnabled;
    public override void InitByLevel(byte level)
    {
        base.InitByLevel(level);
        _elroySpeed = _currentLevelElroySpeed[level < 4? level : 4];
    }
    protected override Vector2 ChaseTile()
    {
        return _playerTransform.position;
    }
    protected override Vector2 _scatterTargetTile => _elroyEnabled ? ChaseTile() : new(11.5f, 16.5f);
    protected override Vector2 _eatenTargetTile => new(-0.5f, -0.5f);
    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.ElroyRemainingPellets += ActivateElroy;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.ElroyRemainingPellets -= ActivateElroy;
    }
    private void ActivateElroy()
    {
        _scatterChaseSpeed = _elroyEnabled? _elroySpeed - 0.006f : _elroySpeed;
        _elroyEnabled = true;
        _eyesSprites = _furiousEyesSprites;
    }
}
