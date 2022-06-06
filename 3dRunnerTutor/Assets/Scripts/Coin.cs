using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private readonly int _speedRotation = 40;
    void Update()
    {
        gameObject.transform.Rotate(0, _speedRotation * Time.deltaTime, 0);
    }
}
