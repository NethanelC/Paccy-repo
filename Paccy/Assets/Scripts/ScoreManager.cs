using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour 
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TextMeshProUGUI _currentScoreText, _highScoreText;
    [SerializeField] private AudioSource _audioSource;
    private int _currentScore, _highScore;
    private bool _reachHighScore;
    private const string _scoreSaveKey = "HighScore";
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _currentScoreText.text = (_currentScore).ToString();
        _highScore = PlayerPrefs.GetInt(_scoreSaveKey);
        _highScoreText.text = _highScore.ToString();
    }
    public void AddScore(int score)
    {
        _currentScoreText.text = (_currentScore += score).ToString();
        if (_currentScore > _highScore)
        {
            NewHighScore();
        }
    }
    private void NewHighScore()
    {
        _highScoreText.text = _currentScore.ToString();
        PlayerPrefs.SetInt(_scoreSaveKey, _currentScore);
        if (!_reachHighScore)
        {
            _audioSource.Play();
            _reachHighScore = true;
        }
    }
}
