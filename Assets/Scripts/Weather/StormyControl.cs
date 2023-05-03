using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormyControl : WeatherControl
{
    private bool isInDarkness;
    public float maxDarknessTime = 2f;
    private float leftDarknessTime;
    void Awake()
    {
        Initiate();
        isInDarkness = true;
        leftDarknessTime = 0.5f;
    }
    public override void WeatherUpdate()
    {
        if(isInDarkness)
        {
            leftDarknessTime -= Time.deltaTime;
            if(leftDarknessTime <= 0 )
            {
                _animator.SetTrigger("lightning");
                isInDarkness = false;
            }
        }
    }
    public void BacktoDarkness()
    {
        isInDarkness = true;
        leftDarknessTime = maxDarknessTime;
    } 
}