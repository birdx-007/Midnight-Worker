using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckPointControl : MonoBehaviour
{
    public Text key;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void Initiate(string k)
    {
        key.text = k;
    }
}
