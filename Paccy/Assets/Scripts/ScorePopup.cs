using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro _popupText;
    [SerializeField] private Color _ghostColor, _fruitColor;
    public enum Type
    {
        Ghost,
        Fruit
    }
    private void Start()
    {
        Destroy(gameObject, 1);
    }
    public void Init(Type type, int score)
    {
        _popupText.color = type == Type.Ghost ? _ghostColor : _fruitColor;
        _popupText.text = score.ToString();
    }
    public void Init(Type type, int score, Vector3 position)
    {
        _popupText.color = type == Type.Ghost ? _ghostColor : _fruitColor;
        _popupText.text = score.ToString();
        transform.position = position;
    }
}
