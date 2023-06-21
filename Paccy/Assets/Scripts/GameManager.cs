using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using DG.Tweening;
using static UnityEngine.SceneManagement.SceneManager;

public class GameManager : MonoBehaviour
{
    public static event Action ElroyRemainingPellets;
    [SerializeField] private Player _pacmanPlayer;
    [SerializeField] private GameObject[] _attemptsImages = new GameObject[3];
    [SerializeField] private Ghost[] _ghostsCollection = new Ghost[4];
    [SerializeField] private SpriteRenderer[] _currentLevelSymbol = new SpriteRenderer[21];
    [SerializeField] private SpriteRenderer[] _symbolCollection = new SpriteRenderer[7];
    [SerializeField] private TextMeshProUGUI _stateText;
    private const byte _maxPellets = 240;
    private byte _currentLevel, _pelletsRemaining, _attemptsLeft;
    private int _collectedSymbols;
    private readonly int[] _currentLevelElroyPellets = { 20, 30, 40, 40, 40, 50, 50, 50, 60, 60, 60, 80, 80, 80, 100, 100, 100, 100, 120 };
    private void Awake()
    {
        _pelletsRemaining = _maxPellets;
        _attemptsLeft = 3;
        ConsumeAttemptAndReturnIfSufficient();
    }
    private void Start()
    {
        if (_currentLevel == 0)
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.GameStart);
            NewLevel();
        }
    }
    private void OnEnable()
    {
        Pellet.EatenPellet += OnEatenPellet;
        Symbol.EatenSymbol += OnEatenSymbol;
        _pacmanPlayer.PacmanDeath += OnDeath;
    }
    private void OnDisable()
    {
        Pellet.EatenPellet -= OnEatenPellet;
        Symbol.EatenSymbol -= OnEatenSymbol;
        _pacmanPlayer.PacmanDeath -= OnDeath;
    }
    private void SetTextReady()
    {
        _stateText.color = Color.yellow;
        _stateText.text = "READY";
        _stateText.gameObject.SetActive(true);
    }
    private void SetTextGameOver()
    {
        _stateText.color = Color.red;
        _stateText.text = "GAME OVER";
        _stateText.gameObject.SetActive(true);
    }
    private void NewLevel()
    {
        _pacmanPlayer.InitByLevel(_currentLevel);
        for(int i = 0; i < _ghostsCollection.Length; ++i)
        {
            _ghostsCollection[i].InitByLevel(_currentLevel);
        }
        NewRound().Forget();
        //LoadScene(GetActiveScene().buildIndex);
    }
    private async UniTaskVoid NewRound()
    {
        _pacmanPlayer.BasePositionAndFacingSide();
        for (int i = 0; i < _ghostsCollection.Length; ++i)
        {
            _ghostsCollection[i].BasePositionAndFacingSide();
        }
        await UniTask.Delay(TimeSpan.FromSeconds(SoundManager.Instance.StartSoundLengthInSeconds));
        _pacmanPlayer.NewRound();
        for (int i = 0; i < _ghostsCollection.Length; ++i)
        {
            _ghostsCollection[i].NewRound();
        }
        _stateText.gameObject.SetActive(false);
    }
    private void OnEatenPellet()
    {
        _pelletsRemaining--;
        if (_pelletsRemaining == 170 || _pelletsRemaining == 70)
        {
            Instantiate(_currentLevelSymbol[_currentLevel], new Vector2(0, -3.5f), Quaternion.identity);
        }
        else if(_pelletsRemaining == _currentLevelElroyPellets[_currentLevel] || _pelletsRemaining == _currentLevelElroyPellets[_currentLevel] * 0.5f)
        {
            ElroyRemainingPellets?.Invoke();
        }
        else if (_pelletsRemaining == 0)
        {
            NewRound().Forget();
        }
    }
    private void OnEatenSymbol()
    {
        _symbolCollection[_collectedSymbols].sprite = _currentLevelSymbol[_currentLevel].sprite;
        _collectedSymbols += 1 % _symbolCollection.Length;  
    }
    private async void OnDeath()
    {
        DOTween.KillAll();
        for (int i = 0; i < _ghostsCollection.Length; i++)
        {
            _ghostsCollection[i].gameObject.SetActive(false);
        }
        SoundManager.Instance.PlaySound(SoundManager.Sound.Death);
        await UniTask.Delay(TimeSpan.FromSeconds(SoundManager.Instance.DeathSoundLengthInSeconds));
        if (ConsumeAttemptAndReturnIfSufficient())
        {
            SetTextReady();
            NewRound().Forget();
            return;
        }
        SetTextGameOver();
    }
    private bool ConsumeAttemptAndReturnIfSufficient()
    {
        if (_attemptsLeft-- > 0)
        {
            _attemptsImages[_attemptsLeft].SetActive(false);
            return true;
        }
        return false;
    }    
}
