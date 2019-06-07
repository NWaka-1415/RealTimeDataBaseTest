using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardField : MonoBehaviour
{
    /// <summary>
    /// 自分の場に出ているカードの数
    /// </summary>
    private int _cardNumber;

    private void Awake()
    {
        _cardNumber = 0;
    }
}