using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
