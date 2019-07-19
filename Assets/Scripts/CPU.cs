using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : Player
{
    private int _flowerIndex = 0;

    private void Start()
    {
        SetCards(isYours: false);
    }
}