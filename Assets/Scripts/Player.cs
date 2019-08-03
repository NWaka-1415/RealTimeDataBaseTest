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
    [SerializeField] protected int playerNumber = 0;

    protected OverAllManager.UserData userData;

    public virtual void Initialize(OverAllManager.UserData userData)
    {
        SetUp(userData);
        SetCards();
    }

    protected void SetUp(OverAllManager.UserData userData)
    {
        this.userData = userData;
        cardField.Initialize();
    }

    protected void SetCards(bool isYours = true)
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
        cardField.PlusCard(selectCard);
        GameSceneManager.Advance();
    }

    /// <summary>
    /// チャレンジをする
    /// </summary>
    public void Challenge()
    {
        GameSceneManager.SetChallenge();
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
        float rand = Random.Range(0f, 1f);
    }

    /// <summary>
    /// パス
    /// </summary>
    public void Pass()
    {
        GameSceneManager.Advance();
    }

    public CardField CardField => cardField;
}