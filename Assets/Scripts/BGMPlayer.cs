using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMType: int
{
    BGM_Menu = 0,
    BGM_Main
}

[RequireComponent(typeof(AudioSource))]
public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance; private AudioSource _audioSource;
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
        _audioSource.loop = true;
        _audioSource.clip = clips[(int)BGMType.BGM_Menu];
    }
    public void PlayBGM(BGMType type)
    {
        if(_audioSource.isPlaying)
        {
            return;
        }
        _audioSource.clip = clips[(int)type];
        _audioSource.Play();
    }
    public void FadeOutBGM()
    {
        StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {
        while(_audioSource.volume > 0)
        {
            _audioSource.volume -= 0.2f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        _audioSource.Stop();
        _audioSource.volume = 1.0f;
    }
}
