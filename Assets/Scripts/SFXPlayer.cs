using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;

    private AudioSource _audioSource;
    public AudioClip clipQTEHit;
    public AudioClip clipQTESucceed;
    public AudioClip clipQTEFail;
    public AudioClip clipPlayerCaught;
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
        _audioSource.clip = clipQTEHit;
    }

    public void PlayQTEHitSFX()
    {
        _audioSource.clip = clipQTEHit;
        _audioSource.Play();
    }
    public void PlayQTESucceedSFX()
    {
        _audioSource.clip = clipQTESucceed;
        _audioSource.Play();
    }
    public void PlayQTEFailSFX()
    {
        _audioSource.clip = clipQTEFail;
        _audioSource.Play();
    }
    public void PlayPlayerCaughtSFX()
    {
        _audioSource.clip = clipPlayerCaught;
        _audioSource.Play();
    }
}
