using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class WeatherFactor
{
    public const float SUNNY_SPEED_FACTOR = 1f;
    public const float WINDY_SPEED_FACTOR = 1.25f;
    public const float STORMY_SPEED_FACTOR = 0.8f;
}

[Serializable]
public enum WeatherState
{
    Sunny = 0,
    Windy,
    Stormy
}

public abstract class WeatherControl : MonoBehaviour
{
    protected Animator _animator;
    void Update()
    {
        WeatherUpdate();
    }
    public void Initiate()
    {
        _animator = GetComponent<Animator>();
    }
    public abstract void WeatherUpdate();
}
