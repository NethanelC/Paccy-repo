using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Ghost
{
    [SerializeField] private Sprite[] _furiousEyesSprites = new Sprite[4];
    private readonly float[] _currentLevelElroySpeed = { 0.126f, 0.112f, 0.112f, 0.112f, 0.1f };
    private float _elroySpeed;
    private bool _elroyEnabled;
    protected override void NewLevel()
    {
        base.NewLevel();
        _eyesSprites = base._eyesSprites;
        if (GameManager.Level < 5)
        {
            _elroySpeed = _currentLevelElroySpeed[GameManager.Level - 1];
        }
        else
        {
            _elroySpeed = _currentLevelElroySpeed[4];
        }
    }
    protected override Vector2 ChaseTile()
    {
        return _playerTransform.position;
    }
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
    protected override Vector2 ScatterTile()
    {
        if (!_elroyEnabled)
        {
            return base.ScatterTile();
        }
        else
        {
            return ChaseTile();
        }
    }
}
