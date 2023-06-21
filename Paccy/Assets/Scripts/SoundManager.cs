using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _gameStartSound, _deathSound, _ghostSound;
    [SerializeField] private AudioClip[] _munchSounds = new AudioClip[2];
    private byte _currentMunchSound;
    public double StartSoundLengthInSeconds => _gameStartSound.length;
    public double DeathSoundLengthInSeconds => _deathSound.length;
    public enum Sound
    {
        GameStart,
        Death,
        Ghost,
        Munch
    }
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
    }
    public void PlaySound(Sound sound)
    {
        _audioSource.PlayOneShot(GetFittingSound(sound));
    }
    private AudioClip GetFittingSound(Sound sound) => sound switch
    {
        Sound.GameStart => _gameStartSound,
        Sound.Death => _deathSound,
        Sound.Ghost => _ghostSound,
        Sound.Munch => _munchSounds[++_currentMunchSound % _munchSounds.Length],
        _ => throw new NotImplementedException(),
    };
}
