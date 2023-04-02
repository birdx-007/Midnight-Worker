using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackgroundControl : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRenderer.material.mainTextureOffset += new Vector2(Mathf.Sqrt(3f)/2f, 0.5f) * Time.deltaTime;
    }
}
