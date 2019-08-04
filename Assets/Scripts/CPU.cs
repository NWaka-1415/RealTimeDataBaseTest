using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPU : Player
{
    [SerializeField] private Text thinkingText = null;
    [SerializeField] private Button selectThisButton = null;
    private bool _isChallenge;

    public override void Initialize(OverAllManager.UserData userData)
    {
        base.Initialize(userData);
        selectThisButton.enabled = false;
        thinkingText.enabled = false;
        _isChallenge = false;
    }

    public void StartThinking()
    {
        thinkingText.enabled = true;
        if (!GameSceneManager.IsChallenge)
        {
            //チャレンジ前
            float rand = Random.Range(0f, 1f);
            //後半の方がチャレンジ確率が上がる
            float per = GameSceneManager.Turn >= 2 ? 0.4f : 0.6f;
            _isChallenge = !(rand <= per);
            if (GameSceneManager.Turn < 1) _isChallenge = false;
        }
    }

    public void Thinking()
    {
        if (GameSceneManager.IsChallenge)
        {
            //チャレンジモード時
            SelectChallengeNum();
        }
        else
        {
            if (_isChallenge) Challenge();
            else SelectRandomCard();
        }
    }

    private void SelectRandomCard()
    {
        bool loop = true;
        while (loop)
        {
            int selectIndex = Random.Range(0, 3);
            selectCard = myCards[selectIndex];
            loop = selectCard.Decided;
        }

        Decide();
        thinkingText.enabled = false;
    }

    private void SelectChallengeNum()
    {
        if (GameSceneManager.FlowerChallengeNumber == 0)
        {
            //チャレンジを宣言したのが自身であればパスは出来ない
            int flowerChallengeNum = Random.Range(GameSceneManager.FlowerChallengeNumber,
                GameSceneManager.MaxFlowerChallengeNumber);
            GameSceneManager.SetChallengeNumber(flowerChallengeNum, playerNumber);
            GameSceneManager.Advance();
        }
        else
        {
            int diff = GameSceneManager.MaxFlowerChallengeNumber - GameSceneManager.FlowerChallengeNumber;
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


        thinkingText.enabled = false;
    }
}