using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPU : Player
{
    [SerializeField] private Text thinkingText = null;
    [SerializeField] private Button selectThisButton = null;
    [SerializeField] private Image selectThisButtonImage = null;

    private bool _isChallenge;

    public override void Initialize(OverAllManager.UserData userData)
    {
        base.Initialize(userData);
        selectThisButton.onClick.AddListener(cardField.OnclickSelect);
        selectThisButton.gameObject.SetActive(false);
        thinkingText.enabled = false;
        _isChallenge = false;
    }

    public void Initialize(OverAllManager.UserData userData, Player mainPlayer)
    {
        Initialize(userData);
        cardField.SetPlayer(mainPlayer);
    }

    public override void ResetThis()
    {
        base.ResetThis();
        selectThisButton.gameObject.SetActive(false);
        thinkingText.enabled = false;
        _isChallenge = false;
    }

    public void StartThinking()
    {
        thinkingText.enabled = true;
        if (GameSceneManager.GameModeType == GameSceneManager.GameModeTypes.PushCardMode)
        {
            //チャレンジ前
            float rand = Random.Range(0f, 1f);
            //後半の方がチャレンジ確率が上がる
            float per = GameSceneManager.Turn >= 2 ? 0.4f : 0.6f;
            _isChallenge = !(rand <= per);
            if (GameSceneManager.Turn < 1) _isChallenge = false;
        }
        else if (GameSceneManager.GameModeType == GameSceneManager.GameModeTypes.SelectCardFromOtherPlayerMode)
        {
            OpenAllMyCards();
        }
    }

    public void Thinking()
    {
        switch (GameSceneManager.GameModeType)
        {
            case GameSceneManager.GameModeTypes.PushCardMode:
                if (_isChallenge) Challenge();
                else SelectRandomCard();
                break;
            case GameSceneManager.GameModeTypes.SelectChallengeNumberMode:
                //チャレンジモード時
                SelectChallengeNum();
                break;
            case GameSceneManager.GameModeTypes.SelectCardFromOtherPlayerMode:
                break;
        }
    }

    private void SelectRandomCard()
    {
        bool loop = true;
        while (loop)
        {
            int selectIndex = Random.Range(0, 4);
            selectCard = myCards[selectIndex];
            loop = selectCard.Decided;
        }

        DecideSetCard();
        thinkingText.enabled = false;
    }

    private void SelectChallengeNum()
    {
//        Debug.Log($"this.Name: {gameObject.name}\nFlowerChallengeNum: {GameSceneManager.FlowerChallengeNumber}");
        if (GameSceneManager.FlowerChallengeNumber == 0)
        {
            //チャレンジを宣言したのが自身であればパスは出来ない
            int flowerChallengeNum = Random.Range(1, GameSceneManager.MaxFlowerChallengeNumber);
            GameSceneManager.SetChallengeNumber(flowerChallengeNum, playerNumber);
            GameSceneManager.Advance();
        }
        else
        {
            int diff = GameSceneManager.MaxFlowerChallengeNumber - GameSceneManager.FlowerChallengeNumber;
            Debug.Log(diff);
            if (diff <= 0)
            {
                Pass();
            }
            else
            {
                float rand = Random.Range(0f, 1f);
                //差が大きければ宣言確率アップ，差が小さければパス確率アップ
                float per = diff >= GameSceneManager.MaxFlowerChallengeNumber / 2 ? 0.7f : 0.3f;
                if (rand <= per)
                {
                    int flowerChallengeNum = Random.Range(GameSceneManager.FlowerChallengeNumber,
                        GameSceneManager.MaxFlowerChallengeNumber);
                    GameSceneManager.SetChallengeNumber(flowerChallengeNum, playerNumber);
                    GameSceneManager.Advance();
                }
                else Pass();
            }
        }


        thinkingText.enabled = false;
    }

    /// <summary>
    /// 他のプレイヤーからカードを選ぶ
    /// </summary>
    private void SelectRandomCardFromOtherPlayer()
    {
        bool loop = true;
        while (loop)
        {
            foreach (CardField field in GameSceneManager.CardFields)
            {
                loop = false;
                if (this.cardField == field || field.CardsCount <= 0) loop = true;
                else selectCardField = field;
            }
        }

        DecideOpenCard();
    }

    public void SetSelectThisButton()
    {
        //権利者が自分だったら無視
        if (GameSceneManager.ChallengePlayerNumber == playerNumber) return;
        selectThisButton.gameObject.SetActive(true);
//        Color color = selectThisButtonImage.color;
//        color = new Color(color.r, color.g, color.b, 0f);
//        selectThisButtonImage.color = color;
    }

    public Image SelectThisButtonImage => selectThisButtonImage;
}