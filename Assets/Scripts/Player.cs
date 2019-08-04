using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 手札
    /// </summary>
    [SerializeField] protected Card[] myCards = new Card[4];

    [SerializeField] protected CardField cardField = null;

    protected Card selectCard;
    private List<int> _throwCardIndex; //捨てられたカードのインデックス
    [SerializeField] protected int playerNumber = 0;

    protected OverAllManager.UserData userData;

    public virtual void Initialize(OverAllManager.UserData userData)
    {
        this.userData = userData;
        cardField.Initialize();
    }

    /// <summary>
    /// カードをセット
    /// </summary>
    /// <param name="isYours"></param>
    public void SetCards(bool isYours = true)
    {
        int myCardSize = myCards.Length;
        for (int i = 0; i < myCardSize - 1; i++)
        {
            myCards[i].Initialize(OverAllManager.Card.CardTypes.Flower,
                GameSceneManager.CardFlowerSprites[playerNumber],
                GameSceneManager.CardBackSprites[playerNumber], isYours);
            myCards[i].SetParentPlayer(this);
            OverAllManager.YourUserData.HavingCards[i] =
                new OverAllManager.Card(myCards[i].CardType, myCards[i].State);
        }

        myCards[myCardSize - 1].Initialize(OverAllManager.Card.CardTypes.Skull,
            GameSceneManager.CardSkullSprites[playerNumber], GameSceneManager.CardBackSprites[playerNumber], isYours);
        myCards[myCardSize - 1].SetParentPlayer(this);
        OverAllManager.YourUserData.HavingCards[myCardSize - 1] =
            new OverAllManager.Card(myCards[myCardSize - 1].CardType, myCards[myCardSize - 1].State);
    }

    public void SetPlayerNumber(int playerNumber)
    {
        this.playerNumber = playerNumber;
    }

    public void Select(Card card)
    {
        selectCard = card;
        foreach (Card myCard in myCards)
        {
            myCard.Unselect();
        }

        selectCard.Select();
//        Debug.Log($"Select:{_selectCard}");
    }

    /// <summary>
    /// 場に出すカードを決定
    /// </summary>
    public void Decide()
    {
        if (selectCard == null) return;
        cardField.PlusCard(selectCard);
        GameSceneManager.Advance();
        selectCard = null;
    }

    /// <summary>
    /// チャレンジをする
    /// </summary>
    public void Challenge()
    {
        GameSceneManager.SetChallenge(playerNumber);
    }

    /// <summary>
    /// チャレンジに勝った時
    /// </summary>
    public void GetPoint()
    {
        userData.AddPoint();
        cardField.GetPoint();
    }

    /// <summary>
    /// カードをランダムで捨てる
    /// 負けた際の挙動
    /// </summary>
    public void ThrowAwayCard()
    {
        bool loop = true;
        int rand = 0;
        while (loop)
        {
            rand = Random.Range(0, 3);
            foreach (int index in _throwCardIndex)
            {
                loop = false;
                if (index == rand) loop = true;
            }
        }

        _throwCardIndex.Add(rand);
    }

    /// <summary>
    /// パス
    /// </summary>
    public void Pass()
    {
        GameSceneManager.Advance();
    }

    public int PlayerNumber => playerNumber;

    public CardField CardField => cardField;
}