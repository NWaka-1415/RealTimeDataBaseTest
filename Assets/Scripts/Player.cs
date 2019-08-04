using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 手札
    /// </summary>
    [SerializeField] protected Card[] myCards = new Card[4];

    [SerializeField] protected CardField cardField = null;

    [SerializeField] private Text pointText = null;

    protected Card selectCard;
    protected CardField selectCardField;

    [SerializeField] protected int playerNumber = 0;

    protected OverAllManager.UserData userData;

    protected int point = 0;

    /// <summary>
    /// スカルが出たか
    /// </summary>
    protected bool isOut;

    /// <summary>
    /// 最終フェーズでカードをすべてめくったか
    /// </summary>
    /// <returns></returns>
    protected bool isClear;

    protected float time;

    public virtual void Initialize(OverAllManager.UserData userData)
    {
        this.userData = userData;
        point = 0;
        time = 1f;
        isOut = false;
        isClear = false;
        selectCard = null;
        selectCardField = null;
        cardField.Initialize();
        cardField.SetParent(this);
    }

    public virtual void ResetThis()
    {
        time = 1f;
        isOut = false;
        isClear = false;
        selectCard = null;
        selectCardField = null;
        cardField.ResetThis();
        foreach (Card myCard in myCards)
        {
            myCard.ResetCard();
        }
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

    public void SelectCard(Card card)
    {
        selectCard = card;
        foreach (Card myCard in myCards)
        {
            myCard.Unselect();
        }

        selectCard.Select();
//        Debug.Log($"Select:{_selectCard}");
    }

    public void SelectField(CardField cardField)
    {
        if (cardField.CardsCount <= 0) return;
        selectCardField = cardField;
        foreach (CardField field in GameSceneManager.CardFields)
        {
            field.Unselect();
        }

        selectCardField.Select();
        DecideOpenCard();
    }

    /// <summary>
    /// 場に出すカードを決定
    /// </summary>
    public void DecideSetCard()
    {
        if (selectCard == null) return;
        cardField.PlusCard(selectCard);
        GameSceneManager.Advance();
        selectCard = null;
    }

    /// <summary>
    /// どこのカードを選ぶか決定
    /// </summary>
    public void DecideOpenCard()
    {
        if (selectCardField == null) return;
        Card openCard = selectCardField.SelectField();
        GameSceneManager.OpenedCardsNumber++;
        if (openCard.CardType == OverAllManager.Card.CardTypes.Skull)
        {
            //スカルを選んだらアウト！
            StartCoroutine(Wait(1.5f, (() =>
            {
                isOut = true;
                StartCoroutine(Wait(1.0f, (ThrowAwayCard)));
            })));
        }
        else
        {
            if (GameSceneManager.OpenedCardsNumber >= GameSceneManager.FlowerChallengeNumber)
            {
                //クリア！
                Debug.Log("Nice!");
                StartCoroutine(Wait(1.5f, (GetPoint)));
            }
        }

        selectCardField = null;
    }

    /// <summary>
    /// 自分が権利者の場合はまずは自分のカードをすべてオープン
    /// </summary>
    public void OpenAllMyCards()
    {
        for (int i = 0; i < cardField.CardsCount; i++)
        {
            selectCardField = cardField;
            DecideOpenCard();
            if (isOut) break;
        }
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
        point++;
        cardField.GetPoint();
        pointText.text = $"{point} / 2";
        isClear = true;
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
            loop = false;
            if (myCards[rand].IsThrow) loop = true;
        }

        myCards[rand].ThrowAway();
    }

    /// <summary>
    /// パス
    /// </summary>
    public void Pass()
    {
        GameSceneManager.Advance();
    }

    IEnumerator Wait(float waitTimeSec, Action action)
    {
        yield return new WaitForSeconds(waitTimeSec);
        action();
    }

    public int PlayerNumber => playerNumber;

    public CardField CardField => cardField;

    public bool IsOut => isOut;

    public bool IsClear => isClear;
}