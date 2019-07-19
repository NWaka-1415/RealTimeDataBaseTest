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
        card.transform.position = this.gameObject.transform.position;
        card.Unselect(); //解除
        _cards.Push(card);
        DisableCards();
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
    
    public Vector3 DropPos => _dropPos[_cards.Count];
}