using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonControl : MonoBehaviour
{
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            _button.interactable = false;
        }
        else
        {
            _button.interactable = true;
        }
    }
}
