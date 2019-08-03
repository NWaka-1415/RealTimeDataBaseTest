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
    [SerializeField] private Stack<Card> cards;

    /// <summary>
    /// 表 is Open
    /// 裏 is Close
    /// </summary>
    private OverAllManager.Card.States _fieldState = OverAllManager.Card.States.Open;

    private Vector3[] _dropPos;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="userData"></param>
    public void Initialize()
    {
        cards = new Stack<Card>();
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
        cards.Push(card);
        int index = 0;
        foreach (Card card1 in cards)
        {
            card1.transform.position = _dropPos[index];
        }

        DisableCards();
    }

    /// <summary>
    /// 場にあるカードをすべて固定
    /// </summary>
    private void DisableCards()
    {
        foreach (Card card in cards)
        {
            card.SetDisable();
        }
    }

    /// <summary>
    /// チャレンジに勝ったとき
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
            Card card = cards.Pop();
            card.Open();
            if (card.CardType == OverAllManager.Card.CardTypes.Skull) skull = true;
        }

        return skull;
    }

    public Vector3 DropPos => _dropPos[cards.Count];
}