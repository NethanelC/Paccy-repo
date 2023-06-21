using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;
using static UnityEngine.Random;

[RequireComponent(typeof(BlueGhost))]
public abstract class Ghost : BaseLevelDependant
{
    [SerializeField] private SpriteRenderer _ghostSpriteRenderer, _eyesSpriteRenderer;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private BlueGhost _frightenMode;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _obstacleLayer; 
    [SerializeField] protected Transform _playerTransform, _tileObjectPrefab;
    [SerializeField] protected Sprite[] _eyesSprites = new Sprite[4];
    private CancellationTokenSource _frightenedCancellationToken = new(), _scatterChaseRotationCancellationToken = new();
    private readonly Dictionary<int, float[]> _levelScatterAndChaseDuration = new()
    {
        {0, new float[] { 7, 20, 7, 20, 5, 20, 5 } },
        {1, new float[] { 7, 20, 7, 20, 5, 1033, 0.01f } },
        {2, new float[] { 7, 20, 7, 20, 5, 1033, 0.01f } },
        {3, new float[] { 7, 20, 7, 20, 5, 1033, 0.01f } },
        {4, new float[] { 7, 20, 5, 20, 5, 1037, 0.01f } }
    };
    private readonly byte[] _currentLevelFrightenedDuration = {6, 5, 4, 3, 2, 5, 2, 2, 1, 5, 2, 1, 1, 3, 1, 1, 0, 1 };
    private readonly byte[] _currentLevelFrightenedBlinks = { 5, 5, 5, 5, 5, 5, 5, 5, 3, 5, 5, 3, 3, 5, 3, 3, 0, 3 };
    private readonly float[] _currentLevelGhostNormalSpeed = { 0.134f, 0.118f, 0.118f, 0.118f, 0.106f };
    private readonly float[] _currentLevelTunnelSpeed = { 0.251f, 0.223f, 0.223f, 0.223f, 0.2f };
    private readonly float[] _currentLevelFrightenedSpeed = { 0.2f, 0.183f, 0.183f, 0.183f, 0.167f };
    private float[] _scatterChaseTimers;
    private float _currentSpeed, _tunnelSpeed, _frightenedSpeed;
    private byte _timerIterations, _frightenedTimer, _frightenedBlinks, _frightenedDuration;
    private bool _inScatter, _onChase, _isEaten, _isRetreating, _isHome, _isFrightenable;
    private Vector2 _currentTargetTile, _lastSideSelected;
    protected float _scatterChaseSpeed;
    private const float _eatenSpeed = 0.04f, _width = 27, _height = 35;
    private List<Vector2> _sides = new(4);
    /*    protected Vector2[] _cantTurnUp =
        { 
            new Vector2(1.5f, -9.5f),
            new Vector2(-1.5f, -9.5f),
            new Vector2(-1.5f, 2.5f),
            new Vector2(1.5f, 2.5f)
        };*/
    private Vector2 _homeTile => transform.position * Vector2.down;
    protected abstract Vector2 ChaseTile();
    protected abstract Vector2 _eatenTargetTile { get; }
    protected abstract Vector2 _scatterTargetTile { get; }
    private Vector2 GetTargetTile()
    {
        if (_isHome)
        {
            _currentSpeed = 0.14f;
            return _homeTile;
        }
        else if (_frightenMode.enabled)
        {
            _currentSpeed = _frightenedSpeed;
            return new Vector2(Range(-_width * 0.5f, _width * 0.5f), Range(-_height * 0.5f, _height * 0.5f));
        }
        else if (_isEaten)
        {
            if ((Vector2)transform.position != _eatenTargetTile)
            {
                return _eatenTargetTile;
            }
            else
            {
                BackFromEaten();
            }
        }
        else if (_onChase)
        {
            _currentSpeed = _scatterChaseSpeed;
            return ChaseTile();
        }
        else if (_inScatter)
        {
            _currentSpeed = _scatterChaseSpeed;
            return _scatterTargetTile;
        }
        return Vector2.zero;
    }
    public override void InitByLevel(byte level)
    {
        if (_isFrightenable = level < 16 || level == 17)
        {
            _frightenedSpeed = _currentLevelFrightenedSpeed[level < 4 ? level : 4];
            _frightenedBlinks = _currentLevelFrightenedBlinks[level];
            _frightenedDuration = (byte)Mathf.Abs((_currentLevelFrightenedDuration[level] - 6) / 6);
            _frightenedTimer = (byte)(_currentLevelFrightenedDuration[level] + (_currentLevelFrightenedBlinks[level] == 5 ? 2 : 1));
        }
        if (level < 4)
        {
            _tunnelSpeed = _currentLevelTunnelSpeed[level];
            _scatterChaseSpeed = _currentLevelGhostNormalSpeed[level];
            _scatterChaseTimers = _levelScatterAndChaseDuration[level];
            return;
        }
        _tunnelSpeed = _currentLevelTunnelSpeed[4];
        _scatterChaseSpeed = _currentLevelGhostNormalSpeed[4];
        _scatterChaseTimers = _levelScatterAndChaseDuration[4];
    }
    public override void NewRound()
    {
        //_home = true;
        _frightenMode.enabled = false;
        _onChase = false;
        _isEaten = false;
        _inScatter = true;
        _lastSideSelected = Vector2.zero;
        _animator.speed = 1;
        StartTimer().Forget();
        Move();
    }
    private void MoveOnceToSide(Vector2 side)
    {
        _lastSideSelected = side;
        if (transform.position.x == 14.5f || transform.position.x == -14.5f)
        {
            transform.DOMoveX(13.5f * -Mathf.Sign(transform.position.x), 0).SetEase(Ease.Linear).OnComplete(Move);
            return;
        }
        transform.DOLocalMove((Vector2)transform.position + side, _currentSpeed).SetEase(Ease.Linear).OnComplete(Move);
    }
    private async UniTaskVoid StartTimer()
    {
        _scatterChaseRotationCancellationToken?.Cancel();
        _scatterChaseRotationCancellationToken.Dispose();
        _scatterChaseRotationCancellationToken = new();
        for (_timerIterations = 0; _timerIterations < 7; _timerIterations++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_scatterChaseTimers[_timerIterations]), cancellationToken: _scatterChaseRotationCancellationToken.Token);
            _inScatter = _onChase ? !_inScatter : _inScatter;
            _onChase = _inScatter ? !_onChase : _onChase;
            _isRetreating = !_frightenMode.enabled;
        }
    }
    public bool TryGetEaten()
    {
        if (!_frightenMode.enabled)
        {
            return false;
        }
        DOTween.Kill(this);
        _frightenMode.enabled = false;
        _collider.enabled = false;
        _isEaten = true;
        _currentSpeed = _eatenSpeed;
        _animator.SetTrigger("Eaten");
        return true;
    }
    public void BackFromEaten()
    {
        _collider.enabled = true;
        _isEaten = false;
        _currentSpeed = _scatterChaseSpeed;
        _animator.SetTrigger("Normal");
    }
    protected virtual void OnEnable()
    {
        BigPellet.EatenBigPellet += Fright;
    }
    protected virtual void OnDisable()
    {
        BigPellet.EatenBigPellet -= Fright;
    }
    private void Move()
    {
        if (_isRetreating)
        {
            _isRetreating = false;
            MoveOnceToSide(-_lastSideSelected);
            return;
        }
        _currentTargetTile = GetTargetTile();
        _tileObjectPrefab.position = _currentTargetTile;
        _sides.Clear();
        _sides.Add(Vector2.up);
        _sides.Add(Vector2.left);
        _sides.Add(Vector2.down);
        _sides.Add(Vector2.right);
        _sides.Remove(-_lastSideSelected);
        _sides.RemoveAll(side => IsObstacle(side));
        if (_sides.Count > 1)
        {
            float lowestDistance = Vector2.Distance(_currentTargetTile, (Vector2)transform.position + _sides[_sides.Count - 1]);
            for (int i = _sides.Count - 1; i >= 0; i--)
            {
                float currentDistance = Vector2.Distance(_currentTargetTile, (Vector2)transform.position + _sides[i]);
                if (currentDistance < lowestDistance)
                {
                    lowestDistance = currentDistance;
                    _sides.RemoveAt(_sides.Count - 1);
                }
                else if (currentDistance > lowestDistance)
                {
                    _sides.RemoveAt(i);
                }
            }
        }
        if (_sides.Contains(Vector2.up))
        {
            _eyesSpriteRenderer.sprite = _eyesSprites[0];
            MoveOnceToSide(Vector2.up);
        }
        else if (_sides.Contains(Vector2.left))
        {
            _eyesSpriteRenderer.sprite = _eyesSprites[1];
            MoveOnceToSide(Vector2.left);
        }
        else if (_sides.Contains(Vector2.down))
        {
            _eyesSpriteRenderer.sprite = _eyesSprites[2];
            MoveOnceToSide(Vector2.down);
        }
        else
        {
            _eyesSpriteRenderer.sprite = _eyesSprites[3];
            MoveOnceToSide(Vector2.right);
        }
    }
    protected bool IsObstacle(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1, _obstacleLayer);
        return hit.collider != null;
    }
    public void Fright()
    {
        if (_isEaten || !_isFrightenable)
        {
            return;
        }
        _frightenMode.enabled = true;
        _isRetreating = true;
        StartFrightTimer().Forget();
        _animator.SetInteger("Blinks", _frightenedBlinks);
        _animator.Play("GhostFright", 0, _frightenedDuration);
    }
    private async UniTaskVoid StartFrightTimer()
    {
        // Cancel previous timer if exists
        _frightenedCancellationToken?.Cancel();
        _frightenedCancellationToken.Dispose();
        _frightenedCancellationToken = new();
        await UniTask.Delay(TimeSpan.FromSeconds(_frightenedTimer), cancellationToken: _frightenedCancellationToken.Token);
        _frightenMode.enabled = false;
    }
}