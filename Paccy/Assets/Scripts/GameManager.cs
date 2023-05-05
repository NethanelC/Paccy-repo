using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _currentLevelSymbol = new SpriteRenderer[21];
    [SerializeField] private SpriteRenderer[] _symbolCollection = new SpriteRenderer[7];
    public static List<GameObject> AllPellets = new();
    public static event Action NoRemainingPellets, ElroyRemainingPellets;
    private static int _currentLevel = 1;
    private int _currentPellets = 240, _collectedSymbols;
    private readonly static int[] _currentLevelElroyPellets = { 20, 30, 40, 40, 40, 50, 50, 50, 60, 60, 60, 80, 80, 80, 100, 100, 100, 100, 120 };
    public static int Level
    {
        get => _currentLevel;
        set => _currentLevel = value;
    }
    private void ReducePellets()
    {
        _currentPellets--;
        if (_currentPellets == 170 || _currentPellets == 70)
        {
            Instantiate(_currentLevelSymbol[Level - 1], new Vector2(0, -3.5f), Quaternion.identity);
        }
        else if(_currentPellets == _currentLevelElroyPellets[Level - 1] || _currentPellets == _currentLevelElroyPellets[Level - 1] * 0.5f)
        {
            ElroyRemainingPellets?.Invoke();
        }
        else if (_currentPellets == 0)
        {
            foreach (GameObject pellet in AllPellets)
            {
                pellet.SetActive(true);
            }
            _currentPellets = 240;
            Level++;
            NoRemainingPellets?.Invoke();
        }
    }
    private void AddSymbol()
    {
        _symbolCollection[_collectedSymbols].sprite = _currentLevelSymbol[Level - 1].sprite;
        _collectedSymbols += 1 % _symbolCollection.Length;  
    }
    private void OnEnable()
    {
        Pellet.EatenPellet += ReducePellets;
        Symbol.EatenSymbol += AddSymbol;
    }
    private void OnDisable()
    {
        Pellet.EatenPellet -= ReducePellets;
        Symbol.EatenSymbol -= AddSymbol;
    }
}
