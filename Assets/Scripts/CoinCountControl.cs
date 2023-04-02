using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCountControl : MonoBehaviour
{
    private Text _text;
    private int _count;
    void Start()
    {
        Initiate();
    }

    void Update()
    {
        _text.text = (_count / 10).ToString() + (_count % 10).ToString();
    }
    public void Initiate()
    {
        _text = GetComponent<Text>();
        _count = 0;
    }
    public void AddOne()
    {
        _count = Math.Clamp(_count + 1, 0, 99);
    }
}
