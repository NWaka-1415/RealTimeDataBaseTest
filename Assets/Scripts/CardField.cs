using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardField : MonoBehaviour
{
    /// <summary>
    /// 置かれているカード
    /// </summary>
    private List<Card> _cards;

    /// <summary>
    /// ターン中に配置されたカード数
    /// </summary>
    private int _turnCardNumber;

    private Vector3[] _dropPos;

    private void Awake()
    {
        _cards = new List<Card>();
        _turnCardNumber = 0;
    }

    /// <summary>
    /// 場に置かれたカード数を加算
    /// </summary>
    public void PlusCard(Card card)
    {
        _cards.Add(card);
        _turnCardNumber++;
    }

    /// <summary>
    /// Challengeによってフィールドが選択されたとき
    /// そのフィールド上のカードは全部表にされる
    /// ついでにスカルが含まれていないかを返却
    /// </summary>
    public bool SelectField()
    {
        bool skull = false;
        foreach (Card card in _cards)
        {
            card.Open();
            if (card.CardType == OverAllManager.Card.CardTypes.Skull) skull = true;
        }

        return skull;
    }

    /// <summary>
    /// ターンが終わるとき
    /// ターン中に配置されたカード数を0にリセット
    /// </summary>
    public void TurnEnd()
    {
        _turnCardNumber = 0;
    }

    public int TurnCardNumber => _turnCardNumber;
}