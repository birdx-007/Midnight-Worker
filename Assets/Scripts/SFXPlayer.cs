using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum SFXType : int
{
    QTE_HIT = 0,
    QTE_SUCCEED,
    QTE_FAIL,
    PLAYER_CAUGHT,
    MENU_BUTTON_PRESSED,
    TRY_CHANGE_LEVEL_SELECTED
}

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;

    private AudioSource _audioSource;
    public AudioClip[] clips;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.clip = clips[(int)SFXType.QTE_HIT];
    }
    public void PlaySFX(SFXType type)
    {
        _audioSource.clip = clips[(int)type];
        _audioSource.Play();
    }
}
