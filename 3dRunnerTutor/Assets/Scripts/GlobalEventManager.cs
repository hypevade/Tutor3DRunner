using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalEventManager : MonoBehaviour
{

    public static UnityEvent OnChunkComplete = new UnityEvent();
    public static UnityEvent OnMoneyAdd = new UnityEvent();


    public static UnityEvent OnGameStart = new UnityEvent();
    public static UnityEvent OnStartPause = new UnityEvent();
    public static UnityEvent OnFinishPause = new UnityEvent();
    public static UnityEvent OnGameOver = new UnityEvent();
}