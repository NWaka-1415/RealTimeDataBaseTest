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

    /// <summary>
    /// カードを置く位置
    /// </summary>
    private Vector3[] _dropPos;

    /// <summary>
    /// カードを開いた後に置く位置
    /// </summary>
    private Vector3[] _openPos;

    /// <summary>
    /// 開いているカードの数
    /// </summary>
    private int _openNumber;

    private Player _player;

    private Player _parent;

    private bool _isSelect;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="userData"></param>
    public void Initialize()
    {
        cards = new Stack<Card>();
        _openNumber = 0;
        Vector3 position = transform.position;
        _isSelect = false;
        _dropPos = new[]
        {
            position + new Vector3(-10, -10),
            position + new Vector3(-5, -5),
            position,
            position + new Vector3(5, 5)
        };
        _openPos = new[]
        {
            position + new Vector3(10f, 20f),
            position + new Vector3(10f, 5f),
            position + new Vector3(10f, -10f),
            position + new Vector3(10f, -25f)
        };
    }

    public void ResetThis()
    {
        cards = new Stack<Card>();
        _openNumber = 0;
        _isSelect = false;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public void SetParent(Player parent)
    {
        _parent = parent;
    }

    public void Select()
    {
        _isSelect = true;
    }

    public void Unselect()
    {
        _isSelect = false;
    }

    /// <summary>
    /// 場に置かれたカード数を加算
    /// </summary>
    public void PlusCard(Card card)
    {
        card.gameObject.SetActive(true);
        card.transform.position = this.gameObject.transform.position;
        card.Unselect(); //解除
        cards.Push(card);

        Stack<Card> cardsTmp = new Stack<Card>();
        int index = 0;
        foreach (Card card1 in cards)
        {
            cardsTmp.Push(cards.Peek());
        }

        foreach (Card card1 in cardsTmp)
        {
            card1.transform.SetSiblingIndex(index);
            card1.transform.position = _dropPos[index];
            index++;
        }

        DisableCards();
    }

    public bool CheckSkull()
    {
        foreach (Card card in cards)
        {
            if (card.CardType == OverAllManager.Card.CardTypes.Skull) return true;
        }

        return false;
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

    public void OnclickSelect()
    {
        _player.SelectField(this);
    }

    /// <summary>
    /// Challengeによってフィールドが選択されたとき
    /// </summary>
    public Card SelectField()
    {
        Card card = cards.Pop();
        card.Open();
        card.transform.position = _openPos[_openNumber];
        _openNumber++;
        return card;
    }

    public Vector3 DropPos => _dropPos[cards.Count];

    public int CardsCount => cards.Count;
}