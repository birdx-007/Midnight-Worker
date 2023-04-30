using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PoliceControl : EnemyControl
{
    void Awake()
    {
        Initiate();
        EnemyBehaviorControl.speedList = new float[] { 0f, 2f, 2f, 3f };
        UpdateMoveable();
        _animator.SetBool("isPoliceman", true);
    }
}
