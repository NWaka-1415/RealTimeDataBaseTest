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

    /// <summary>
    /// 場に置かれたカード数を加算
    /// </summary>
    public void PlusCard()
    {
        _cardNumber++;
    }

    public int CardNumber => _cardNumber;
}