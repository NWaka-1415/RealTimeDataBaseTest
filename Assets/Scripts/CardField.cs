using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardField : MonoBehaviour
{
    /// <summary>
    /// 置かれているカード
    /// </summary>
    [SerializeField] private Stack<Card> _cards;

    /// <summary>
    /// ターン中に配置されたカード数
    /// </summary>
    private int _turnCardNumber;

    /// <summary>
    /// 表 is Open
    /// 裏 is Close
    /// </summary>
    private OverAllManager.Card.States _fieldState = OverAllManager.Card.States.Open;

    private OverAllManager.UserData _userData;

    private Vector3[] _dropPos;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="userData"></param>
    public void Initialize(OverAllManager.UserData userData)
    {
        _userData = userData;
        _cards = new Stack<Card>();
        _turnCardNumber = 0;
        _dropPos = new[]
        {
            transform.position + new Vector3(-10, -10),
            transform.position + new Vector3(-5, -5),
            transform.position,
            transform.position + new Vector3(5, 5)
        };
    }

    /// <summary>
    /// 場に置かれたカード数を加算
    /// </summary>
    public void PlusCard(Card card)
    {
        _cards.Push(card);
        _turnCardNumber++;
    }

    /// <summary>
    /// 場にあるカードをすべて固定
    /// </summary>
    private void DisableCards()
    {
        foreach (Card card in _cards)
        {
            card.SetDisable();
        }
    }

    /// <summary>
    /// プレイヤーがチャレンジに勝ったとき
    /// </summary>
    public void GetPoint()
    {
        switch (_fieldState)
        {
            case OverAllManager.Card.States.Open:
                _fieldState = OverAllManager.Card.States.Close;
                break;
            case OverAllManager.Card.States.Close:
                break;
        }

        _userData.AddPoint();
    }

    /// <summary>
    /// Challengeによってフィールドが選択されたとき
    /// そのフィールド上のカードは全部表にされる
    /// ついでにスカルが含まれていないかを返却
    /// </summary>
    public bool SelectField(int number)
    {
        bool skull = false;
        for (int i = 0; i < number; i++)
        {
            Card card = _cards.Pop();
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
        DisableCards();
    }

    public Vector3 DropPos => _dropPos[_cards.Count];

    public int TurnCardNumber => _turnCardNumber;
}