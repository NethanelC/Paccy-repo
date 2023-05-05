using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource, _highScoreAudioSource, _currentModeAudioSource;
    [SerializeField] private GameObject[] _attemptsImages = new GameObject[2];
    [SerializeField] private Ghost[] _ghosts = new Ghost[4];
    [SerializeField] private AudioClip[] _sounds = new AudioClip[7];
    [SerializeField] private TextMeshProUGUI _currentScoreText, _highScoreText, _gameStateText;
    [SerializeField] private TextMeshPro _scoreObject;
    [SerializeField] private LayerMask _terrainLayer;
    private Transform _transform;
    private float[] _currentLevelPacmanNormalSpeed = new float[5];
    private int _attemptLives = 2, _currentScore, _highScore, _frightEaten = 200, _currentChompSound;
    private bool _reachHighScore;
    private Vector2 _currentDirection, _nextDirection, _startPosition;
    private void Awake()
    {
        _transform = transform;
        _startPosition = _transform.position;
        _highScore = PlayerPrefs.GetInt("HS");
        _highScoreText.text = $"{_highScore}";
    }
    private void Start()
    {   
        PlaySound(_sounds[4]);
        StartCoroutine(GameStart());
    }
    private void Update()
    {
        bool wUpKey = Input.GetKeyDown(KeyCode.W);
        bool arrowUpKey = Input.GetKeyDown(KeyCode.UpArrow);
        bool sDownKey = Input.GetKeyDown(KeyCode.S);
        bool arrowDownKey = Input.GetKeyDown(KeyCode.DownArrow);
        bool dRightKey = Input.GetKeyDown(KeyCode.D);
        bool arrowRightKey = Input.GetKeyDown(KeyCode.RightArrow);
        bool aLeftKey = Input.GetKeyDown(KeyCode.A);
        bool arrowLeftKey = Input.GetKeyDown(KeyCode.LeftArrow);
        if (wUpKey || arrowUpKey)
        {
            SetDirection(Vector2.up);
        }
        else if (sDownKey || arrowDownKey)
        {
            SetDirection(Vector2.down);
        }
        else if (aLeftKey || arrowLeftKey)
        {
            SetDirection(Vector2.left);
        }
        else if (dRightKey || arrowRightKey)
        {
            SetDirection(Vector2.right);
        }
        if (_nextDirection != Vector2.zero)
        {
            SetDirection (_nextDirection);
        }
    }
    private void FixedUpdate()
    {
        Vector2 thing = (9 * Time.fixedDeltaTime * _currentDirection);
        _rigidBody.MovePosition(_rigidBody.position + thing);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            EatPellet();
        }
        else if (collision.gameObject.layer == 7)
        {
            AddScore(50);
            _currentModeAudioSource.Play();
            if (GameManager.Level < 17 || GameManager.Level == 18)
            {
                _frightEaten = 200;
            }
        }
        else if (collision.gameObject.layer == 8)
        {
            StartCoroutine(Death());
        }
        else if (collision.gameObject.layer == 12)
        {
            collision.gameObject.GetComponent<Ghost>().Eaten();
            StartCoroutine(EatGhost());
        }
    }
    private void SetDirection(Vector2 direction)
    {
        if (!IsObstacle(direction))
        {
            _currentDirection = direction;
            _nextDirection = Vector2.zero;
            float angle = Mathf.Atan2(direction.y, direction.x);
            _transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }
        else
        {
            _nextDirection = direction;
        }
    }
    private bool IsObstacle(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(_transform.position, Vector2.one * 0.75f, 0, direction, 1.5f, _terrainLayer);
        return hit.collider != null;
    }    
    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
    private void AddScore(int score)
    {
        _currentScoreText.text = $"{_currentScore += score}";
        if (_currentScore > _highScore)
        {
            _highScoreText.text = $"{_currentScore}";
            PlayerPrefs.SetInt("HS", _currentScore);
            if (!_highScoreAudioSource.isPlaying && !_reachHighScore)
            {
                _highScoreAudioSource.Play();
                _reachHighScore = true;
            }  
        }
    }
    private IEnumerator GameStart()
    {
        _animator.speed = 0;
        _animator.Play("PacmanEat", 0, 0);
        _gameStateText.color = Color.yellow;
        _gameStateText.text = $"READY";
        _gameStateText.gameObject.SetActive(true);
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        _transform.position = _startPosition;
        SetDirection(Vector2.left);
        yield return new WaitForSeconds(4.5f);
        _animator.speed = 1;
        _gameStateText.gameObject.SetActive(false);
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    private void EatPellet()
    {
        AddScore(10);
        if (_currentChompSound == 0)
        {
            PlaySound(_sounds[0]);
            _currentChompSound++;
        }
        else if (_currentChompSound == 1)
        {
            PlaySound(_sounds[1]);
            _currentChompSound--;
        }  
    }
    private IEnumerator EatGhost()
    {
        PlaySound(_sounds[2]);
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        _currentModeAudioSource.Pause();
        DOTween.PauseAll();
        yield return new WaitForSeconds(0.5f);
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        DOTween.PlayAll();
        _currentModeAudioSource.Play();
        AddScore(_frightEaten);
        var yes = Instantiate(_scoreObject, _transform.position, Quaternion.identity);
        yes.text = $"{_frightEaten}";
        Destroy(yes, 1);
        _frightEaten *= 2;
    }
    private IEnumerator Death()
    {
        DOTween.KillAll();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        _currentModeAudioSource.Pause();
        yield return new WaitForSeconds(1);
        PlaySound(_sounds[3]);
        _animator.SetTrigger("Death");
        _attemptLives--;
        foreach (Ghost ghost in _ghosts)
        {
            ghost.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1.5f);
        if (_attemptLives > -1)
        {
            Destroy(_attemptsImages[_attemptLives]);
            foreach (Ghost ghost in _ghosts)
            {
                StartCoroutine(ghost.GameStart());
            }
            StartCoroutine(GameStart());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            _gameStateText.color = Color.red;
            _gameStateText.text = $"GAME OVER";
            _gameStateText.gameObject.SetActive(true);
        }
    }
}
