using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 手札
    /// </summary>
    [SerializeField] Card[] _myCards = new Card[4];

    [SerializeField] private CardField _cardField = null;

    private Card _selectCard;
    [SerializeField] private int _playerNumber = 0;

    // Start is called before the first frame update
    private void Start()
    {
        SetCards();
    }

    protected void SetCards(bool isYours = true)
    {
        int myCardSize = _myCards.Length;
        for (int i = 0; i < myCardSize - 1; i++)
        {
            _myCards[i].Initialize(OverAllManager.Card.CardTypes.Flower,
                GameSceneManager.CardFlowerSprites[_playerNumber],
                GameSceneManager.CardBackSprites[_playerNumber], isYours);
            _myCards[i].SetParentPlayer(this);
            OverAllManager.YourUserData.HavingCards[i] =
                new OverAllManager.Card(_myCards[i].CardType, _myCards[i].State);
        }

        _myCards[myCardSize - 1].Initialize(OverAllManager.Card.CardTypes.Skull,
            GameSceneManager.CardSkullSprites[_playerNumber], GameSceneManager.CardBackSprites[_playerNumber], isYours);
        _myCards[myCardSize - 1].SetParentPlayer(this);
        OverAllManager.YourUserData.HavingCards[myCardSize - 1] =
            new OverAllManager.Card(_myCards[myCardSize - 1].CardType, _myCards[myCardSize - 1].State);
    }

    public void Select(Card card)
    {
        _selectCard = card;
        foreach (Card myCard in _myCards)
        {
            myCard.Unselect();
        }

        _selectCard.Select();
        Debug.Log($"Select:{_selectCard}");
    }

    /// <summary>
    /// 場に出すカードを決定
    /// </summary>
    public void Decide()
    {
        _cardField.PlusCard(_selectCard);
        GameSceneManager.Advance();
    }

    /// <summary>
    /// チャレンジをする
    /// </summary>
    public void Challenge()
    {
        GameSceneManager.SetChallenge();
    }

    public void TurnEnd()
    {
        GameSceneManager.AdvanceTurn();
    }

    public CardField CardField => _cardField;
}