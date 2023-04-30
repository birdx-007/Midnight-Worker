using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class WatchmanControl : EnemyControl
{
    void Awake()
    {
        Initiate();
        EnemyBehaviorControl.speedList = new float[] { 0f, 1f, 1f, 1.6f };
        UpdateMoveable();
        _animator.SetBool("isWatchman", true);
    }
}
