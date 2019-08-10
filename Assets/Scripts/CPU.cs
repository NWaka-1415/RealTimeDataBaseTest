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

    public void StartThinking(GameSceneManager.GameModeTypes gameModeType)
    {
        thinkingText.enabled = true;
        if (gameModeType == GameSceneManager.GameModeTypes.PushCardMode)
        {
            //チャレンジ前
            float rand = Random.Range(0f, 1f);
            //後半の方がチャレンジ確率が上がる
            float per = GameSceneManager.Turn >= 2 ? 0.4f : 0.6f;
            //スカルが自分の場に含まれている場合はチャレンジ確率を大幅に下げる
            per = cardField.CheckSkull() ? 0.05f : per;
            _isChallenge = !(rand <= per);
            if (GameSceneManager.Turn < 1) _isChallenge = false;
        }
        else if (gameModeType == GameSceneManager.GameModeTypes.SelectCardFromOtherPlayerMode)
        {
            Debug.Log("CPU OpenMyCards");
            OpenAllMyCards();
        }
    }

    public void Thinking(GameSceneManager.GameModeTypes gameModeType)
    {
        switch (gameModeType)
        {
            case GameSceneManager.GameModeTypes.PushCardMode:
                if (_isChallenge) Challenge();
                else SelectRandomCard();
                break;
            case GameSceneManager.GameModeTypes.SelectChallengeNumberMode:
                //チャレンジモード時
                Debug.Log("OK!");
                SelectChallengeNum();
                break;
            case GameSceneManager.GameModeTypes.SelectCardFromOtherPlayerMode:
                Debug.Log("Enter While 1");
                while (!isClear && !isOut)
                {
                    Debug.Log($"!isClear:{!isClear},!isOut:{!isOut}\n&&:{!isClear && !isOut}");
                    SelectRandomCardFromOtherPlayer();
                }

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
            loop = selectCard.Decided || selectCard.IsThrow;
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
                    int flowerChallengeNum = Random.Range(GameSceneManager.FlowerChallengeNumber + 1,
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
    /// もし自分やオープンしていないFieldを選んだ場合はやり直し
    /// </summary>
    private void SelectRandomCardFromOtherPlayer()
    {
        List<CardField> selectCardFields = new List<CardField>();
        selectCardField = null;
        for (int i = 0; i < GameSceneManager.FlowerChallengeNumber - GameSceneManager.OpenedCardsNumber; i++)
        {
            while (true)
            {
                int rand = Random.Range(0, GameSceneManager.CardFields.Count);
                CardField field = GameSceneManager.CardFields[rand];
                Debug.Log($"if({cardField == field || field.CardsCount <= 0})");
                if (!(cardField == field || field.CardsCount <= 0))
                {
                    selectCardFields.Add(field);
                    break;
                }
            }
        }

        Debug.Log($"CardFieldNum: {selectCardFields.Count}");

//        while (true)
//        {
//            int rand = Random.Range(0, GameSceneManager.CardFields.Count);
//            CardField field = GameSceneManager.CardFields[rand];
//            Debug.Log($"if({cardField == field || field.CardsCount <= 0})");
//            if (!(cardField == field || field.CardsCount <= 0))
//            {
//                selectCardField = field;
//                break;
//            }
//        }

        foreach (CardField field in selectCardFields)
        {
            selectCardField = field;
            DecideOpenCard();
        }
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