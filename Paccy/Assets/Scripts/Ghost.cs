using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Ghost : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _ghostSpriteRenderer, _eyesSpriteRenderer;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _terrainLayer;
    [SerializeField] protected Vector2 _scatterTargetTile, _eattenTargetTile;
    [SerializeField] protected Transform _playerTransform, _tileObjectPrefab;
    [SerializeField] protected Sprite[] _eyesSprites = new Sprite[4];
    private readonly float[][] _levelScatterAndChaseDuration =
    {
    new float[] { 7, 20, 7, 20, 5, 20, 5 },
    new float[] { 7, 20, 7, 20, 5, 1033, 0.01f },
    new float[] { 7, 20, 7, 20, 5, 1033, 0.01f },
    new float[] { 7, 20, 7, 20, 5, 1033, 0.01f },
    new float[] { 7, 20, 5, 20, 5, 1037, 0.01f }
    };
    private readonly float[] _currentLevelGhostNormalSpeed = { 0.134f, 0.118f, 0.118f, 0.118f, 0.106f };
    private readonly float[] _currentLevelTunnelSpeed = { 0.251f, 0.223f, 0.223f, 0.223f, 0.2f };
    private readonly float[] _currentLevelFrightenedSpeed = { 0.2f, 0.183f, 0.183f, 0.183f, 0.167f };
    private readonly float[] _currentLevelFrightenedDuration = {6, 5, 4, 3, 2, 5, 2, 2, 1, 5, 2, 1, 1, 3, 1, 1, 0, 1 };
    private readonly int[] _currentLevelFrightenedBlinks = { 5, 5, 5, 5, 5, 5, 5, 5, 3, 5, 5, 3, 3, 5, 3, 3, 0, 3 };
    private float _timerStartTime, _currentSpeed, _tunnelSpeed, _scatterChaseTimer, _frightenedTimer, _frightenedSpeed;
    private bool _timerRunning, _scatter, _chase, _frightened, _eaten, _retreat, _home; 
    private int currentTimer;
    private Vector2 _targetTile, _cantMoveTo, _startDirection, _startPosition;
    protected float  _scatterChaseSpeed;
    protected List <Vector2> _sides = new();
    protected Vector2[] _cantTurnUp =
        { 
        new Vector2(1.5f, -9.5f),
        new Vector2(-1.5f, -9.5f),
        new Vector2(-1.5f, 2.5f),
        new Vector2(1.5f, 2.5f)
        };
    protected virtual void NewLevel()
    {
        if (GameManager.Level < 5)
        {
            _tunnelSpeed = _currentLevelTunnelSpeed[GameManager.Level - 1];
            _scatterChaseSpeed = _currentLevelGhostNormalSpeed[GameManager.Level - 1];
            _scatterChaseTimer = GameManager.Level - 1;
            _frightenedSpeed = _currentLevelFrightenedSpeed[GameManager.Level - 1];
        }
        else
        {  
            _tunnelSpeed = _currentLevelTunnelSpeed[4];
            _scatterChaseSpeed = _currentLevelGhostNormalSpeed[4];
            _scatterChaseTimer = 4;
            _frightenedSpeed = _currentLevelFrightenedSpeed[4];
        }
    }
        void StartTimer()
        {
            _timerStartTime = Time.time;
            _timerRunning = true;
        }
        void TimerComplete()
        {
            _timerRunning = false;
            // Do something here when the timer completes
            _scatter = _chase? !_scatter : _scatter;
            _chase = _scatter? !_chase : _chase;
            _retreat = true;
            // Move to the next timer
            currentTimer++;
            // Check if there are more timers
            if (currentTimer < _levelScatterAndChaseDuration[(int)_scatterChaseTimer].Length)
            {
                StartTimer();
            }
        }
    public IEnumerator GameStart()
    {
        _chase = false;
        _frightened = false;
        _eaten = false;
        _scatter = false;
        _timerRunning = false;
        currentTimer = 0;
        _animator.speed = 0;
        gameObject.SetActive(true);
        transform.position = _startPosition;
        _cantMoveTo = Vector2.right;
        yield return new WaitForSeconds(4.5f);
        _animator.speed = 1;
        _scatter = true;
        StartTimer();
        Move();
    }
    private void Awake()
    {
        _startPosition = transform.position;
        NewLevel();
    }
    private void Start()
    {
        //_home = true;
        StartCoroutine(GameStart());
    }
    private void Update()
    {   
        if (_frightenedTimer > 0)
        {
            _frightenedTimer -= Time.deltaTime;
        }
        else
        {
            _frightened = false;
            gameObject.layer = 8;
        }
        if (_timerRunning)
        {
            if (Time.time - _timerStartTime >= _levelScatterAndChaseDuration[(int)_scatterChaseTimer][currentTimer])
            {
                TimerComplete();
            }
        }
    }
    protected virtual void OnEnable()
    {
        BigPellet.EatenBigPellet += Fright;
    }
    protected virtual void OnDisable()
    {
        BigPellet.EatenBigPellet -= Fright;
    }
    protected bool IsObstacle(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1, _terrainLayer);
        return hit.collider != null;
    }
    private void Move()
    {
        _targetTile = ChangeTargetTile();
        _tileObjectPrefab.position = _targetTile;
        _sides.Clear();
        _sides.Add(Vector2.up);
        _sides.Add(Vector2.left);
        _sides.Add(Vector2.down);
        _sides.Add(Vector2.right);
        _sides.Remove(_cantMoveTo);
        for (int i = _sides.Count - 1; i >= 0; i--)
        {
            if (IsObstacle(_sides[i]))
            {
                _sides.RemoveAt(i);
                continue;
            }
        }
        if (_sides.Count > 1)
        {
            float initialDistance = Vector2.Distance(_targetTile, new Vector2(transform.position.x, transform.position.y) + _sides[_sides.Count -1]);
            for (int i = _sides.Count - 1; i >= 0; i--)
            {
                float currentDistance = Vector2.Distance(_targetTile, new Vector2(transform.position.x, transform.position.y) + _sides[i]);
                if (currentDistance < initialDistance)
                {
                    initialDistance = currentDistance;
                    _sides.RemoveAt(_sides.Count - 1);
                    continue;
                }
                else if (currentDistance > initialDistance)
                {
                    _sides.RemoveAt(i);
                    continue;
                }
                else if (currentDistance == initialDistance)
                {
                    continue;
                }
            }
        }
        if (_sides.Contains(Vector2.up))
        {
            _eyesSpriteRenderer.sprite = _eyesSprites[0];
            transform.DOLocalMove(new Vector2(transform.position.x, transform.position.y) + Vector2.up, _currentSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                _cantMoveTo = _retreat? Vector2.up: Vector2.down;
                _retreat = _retreat ? !_retreat : _retreat;
                Move();
            });
        }
        else if (_sides.Contains(Vector2.left))
        {
            _eyesSpriteRenderer.sprite = _eyesSprites[1];
            if (transform.position.x < -14)
            {
                transform.DOMoveX(transform.position.x + 27, 0).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _cantMoveTo = _retreat ? Vector2.left : Vector2.right;
                    _retreat = _retreat ? !_retreat : _retreat;
                    Move();
                });
            }
            else
            {
                transform.DOLocalMove(new Vector2(transform.position.x, transform.position.y) + Vector2.left, _currentSpeed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _cantMoveTo = _retreat ? Vector2.left : Vector2.right;
                    _retreat = _retreat ? !_retreat : _retreat;
                    Move();
                });
            }
        }
        else if (_sides.Contains(Vector2.down))
        {
            _eyesSpriteRenderer.sprite = _eyesSprites[2];
            transform.DOLocalMove(new Vector2(transform.position.x, transform.position.y) + Vector2.down, _currentSpeed).SetEase(Ease.Linear).OnComplete(() =>
            {
                _cantMoveTo = _retreat ? Vector2.down : Vector2.up;
                _retreat = _retreat ? !_retreat : _retreat;
                Move();
            });
        }
        else if (_sides.Contains(Vector2.right))
        {
            _eyesSpriteRenderer.sprite = _eyesSprites[3];
            if (transform.position.x > 14)
            {
                transform.DOMoveX(transform.position.x - 27, 0).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _cantMoveTo = _retreat ? Vector2.right : Vector2.left;
                    _retreat = _retreat ? !_retreat : _retreat;
                    Move();
                });
            }
            else
            {
                transform.DOLocalMove(new Vector2(transform.position.x, transform.position.y) + Vector2.right, _currentSpeed).SetEase(Ease.Linear).OnComplete(() =>
                {
                    _cantMoveTo = _retreat ? Vector2.right : Vector2.left;
                    _retreat = _retreat ? !_retreat : _retreat;
                    Move();
                });
            }
        }
    }
    private Vector2 ChangeTargetTile()
    {
        if (_home)
        {
            _cantMoveTo -= _cantMoveTo;
            _currentSpeed = 0.14f;
            return new Vector2(transform.position.x, -transform.position.y);
        }
        if (_frightened)
        {
            _currentSpeed = _frightenedSpeed;
            return new(Random.Range(-13.5f, 13.51f), Random.Range(-17.5f, 17.51f));
        }
        else if(_eaten)
        {
            if (new Vector2(transform.position.x, transform.position.y) != _eattenTargetTile)
            {
                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                } 
                return _eattenTargetTile;
            }
            else
            {
                _audioSource.Stop();
                _eaten = false;
/*                _home = true;*/
                _collider.enabled = true;
                _currentSpeed = _scatterChaseSpeed;
                gameObject.layer = 8;
                _animator.SetTrigger("Normal");
            }
        }
        else if (_chase)
        {
            _currentSpeed = _scatterChaseSpeed;
            return ChaseTile();
        }
        else if (_scatter)
        {
            _currentSpeed = _scatterChaseSpeed;
            return ScatterTile();
        }
        return Vector2.zero;
    }
    protected abstract Vector2 ChaseTile();
    protected virtual Vector2 ScatterTile()
    {
        return _scatterTargetTile;
    }
    public void Eaten()
    {
        _frightened = false;
        _eaten = true;
        _currentSpeed = 0.04f;
        _collider.enabled = false;
        _animator.SetTrigger("Eaten");
    }
    public void Fright()
    {
        if (!_eaten)
        { 
            _frightened = true;
            _retreat = true;
            gameObject.layer = 12;
            _frightenedTimer = _currentLevelFrightenedDuration[GameManager.Level - 1] + (_currentLevelFrightenedBlinks[GameManager.Level - 1] == 5? 2 : 1);
            _animator.SetInteger("Blinks", _currentLevelFrightenedBlinks[GameManager.Level - 1]);
            _animator.Play("GhostFright", 0, Mathf.Abs((_currentLevelFrightenedDuration[GameManager.Level - 1] - 6) / 6));
        }
    }
}