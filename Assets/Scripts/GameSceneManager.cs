using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    /// <summary>
    /// 現在行動可能なプレイヤーの番号
    /// Player number which is able to action
    /// </summary>
    private static int _activeUserNumber;

    /// <summary>
    /// チャレンジ権を持つプレイヤーナンバー
    /// </summary>
    private static int _challengePlayerNumber;

    /// <summary>
    /// チャレンジを宣言したプレイヤーナンバー
    /// </summary>
    private static int _sayChallengePlayerNumber;

    /// <summary>
    /// あなたのプレイヤーの番号
    /// Your player number
    /// </summary>
    private int _yourNumber;

    /// <summary>
    /// 現在のターン数
    /// Turn number on now
    /// </summary>
    private static int _turn;

    public static readonly Sprite[] CardSkullSprites = new Sprite[6];
    public static readonly Sprite[] CardFlowerSprites = new Sprite[6];
    public static readonly Sprite[] CardBackSprites = new Sprite[6];

    [SerializeField] private GameObject bottomCard = null;
    [SerializeField] private GameObject bottomChallenge = null;

    [SerializeField] private Text challengeNumberText = null;
    [SerializeField] private Text selectNumber = null;
    [SerializeField] private Slider challengeNumberSetSlider = null;

    [SerializeField] private Button challengeButton = null;
    [SerializeField] private Button okButtonOnCards = null;

    [SerializeField] private Button okButtonOnChallenge = null;
    [SerializeField] private Button passButton = null;

    [SerializeField] private Player player = null;

    [SerializeField] private CPU[] cpus = new CPU[5];

    /// <summary>
    /// チャレンジのターンか
    /// Is the turn challenge turn?
    /// Yes=>true
    /// No=>false
    /// </summary>
    private static bool _isChallenge;

    private bool _prevChallenge;

    /// <summary>
    /// 宣言されているカード枚数
    /// </summary>
    private static int _flowerChallengeNumber;

    /// <summary>
    /// チャレンジできる最大カード枚数
    /// </summary>
    private static int _maxFlowerChallengeNumber;

    /// <summary>
    /// ターン間の待ち時間(CPU)
    /// </summary>
    private float _timeToTurnAndTurn;

    private void Awake()
    {
        Initialize();
        
        _activeUserNumber = 0;
        _turn = 0;
        
        _isChallenge = false;
        _prevChallenge = _isChallenge;
        challengeButton.interactable = false;

        //リソース読み込み
        for (int i = 0; i < 6; i++)
        {
            Sprite[] cards = Resources.LoadAll<Sprite>($"Cards/Card{i}");
            if (cards == null)
            {
                Debug.LogError("リソース読み込み失敗");
                return;
            }

            CardFlowerSprites[i] = cards[0];
            CardBackSprites[i] = cards[2];
            CardSkullSprites[i] = cards[1];
        }

//        _overAllManager = GameObject.FindWithTag(OverAllManager.TagNames.GameController).GetComponent<OverAllManager>();
        for (int i = OverAllManager.PlayerNumber - 1; i < cpus.Length; i++)
        {
            // 不要なCPU分の場をHide
            cpus[i].gameObject.SetActive(false);
        }

        player.Initialize(OverAllManager.YourUserData);
        for (int i = 0; i < OverAllManager.PlayerNumber - 1; i++)
        {
            cpus[i].Initialize(new OverAllManager.UserData($"hoge{i}", $"hoge{i}"));
        }

        SetPlayer();
        player.SetCards();
        for (int i = 0; i < OverAllManager.PlayerNumber - 1; i++)
        {
            cpus[i].SetCards(false);
        }

        okButtonOnCards.onClick.AddListener(player.Decide);
        challengeButton.onClick.AddListener(player.Challenge);

        okButtonOnChallenge.onClick.AddListener(Select);
        passButton.onClick.AddListener(player.Pass);
    }

    private void Update()
    {
        if (_isChallenge)
        {
            if (_activeUserNumber == _challengePlayerNumber)
            {
                //チャレンジ権利を持つプレイヤーに準場が回ってきた際
                if (_activeUserNumber != 0)
                {
                    //CPUs
                    
                }
            }
            
            if (_prevChallenge != _isChallenge)
            {
                Challenge();
                _prevChallenge = _isChallenge;
            }

            //チャレンジした後の動作
            //ボタンの無効化有効化
            okButtonOnChallenge.interactable = _activeUserNumber == 0;
            passButton.interactable = _activeUserNumber == 0;
            challengeNumberSetSlider.interactable = _activeUserNumber == 0;
            challengeNumberText.text = $"{challengeNumberSetSlider.value}枚";
            if (_activeUserNumber != 0)
            {
                //CPUs
                if (_timeToTurnAndTurn <= 0f)
                {
                    FindActiveCPU(_activeUserNumber).Thinking();
                    selectNumber.text = $"{_flowerChallengeNumber}枚";
                    SetSlider(1, _flowerChallengeNumber + 1, _maxFlowerChallengeNumber);
                    _timeToTurnAndTurn = 1f;
                }
                else if (_timeToTurnAndTurn >= 1f)
                {
                    FindActiveCPU(_activeUserNumber).StartThinking();
                    _timeToTurnAndTurn -= Time.deltaTime;
                }
                else
                {
                    _timeToTurnAndTurn -= Time.deltaTime;
                }
            }
        }
        else
        {
            //ボタンの無効化有効化
            if (_turn > 0) challengeButton.interactable = _activeUserNumber == 0;
            okButtonOnCards.interactable = (_activeUserNumber == 0) && (player.CardField.CardsCount < 4);
            if (_activeUserNumber != 0)
            {
                //CPUs
                if (_timeToTurnAndTurn <= 0f)
                {
                    FindActiveCPU(_activeUserNumber)?.Thinking();
                    _timeToTurnAndTurn = 1f;
                }
                else if (_timeToTurnAndTurn >= 1f)
                {
                    FindActiveCPU(_activeUserNumber)?.StartThinking();
                    _timeToTurnAndTurn -= Time.deltaTime;
                }
                else
                {
                    _timeToTurnAndTurn -= Time.deltaTime;
                }
            }
        }
    }

    private void Initialize()
    {
        _timeToTurnAndTurn = 1f;
        _flowerChallengeNumber = 0;
        _challengePlayerNumber = -1;
        bottomCard.SetActive(true);
        bottomChallenge.SetActive(false);
    }

    private void SetPlayer()
    {
        switch (OverAllManager.PlayerNumber)
        {
            //どの場合もPlayerが0番
            default:
                //3人想定
                cpus[1].SetPlayerNumber(1);
                cpus[0].SetPlayerNumber(2);
                break;
            case 4:
                cpus[1].SetPlayerNumber(1);
                cpus[2].SetPlayerNumber(2);
                cpus[0].SetPlayerNumber(3);
                break;
            case 5:
                cpus[1].SetPlayerNumber(1);
                cpus[2].SetPlayerNumber(2);
                cpus[3].SetPlayerNumber(3);
                cpus[0].SetPlayerNumber(4);
                break;
            case 6:
                cpus[1].SetPlayerNumber(1);
                cpus[4].SetPlayerNumber(2);
                cpus[2].SetPlayerNumber(3);
                cpus[3].SetPlayerNumber(4);
                cpus[0].SetPlayerNumber(5);
                break;
        }
    }

    private CPU FindActiveCPU(int activeNumber)
    {
        foreach (CPU cpu in cpus)
        {
            if (cpu.PlayerNumber == activeNumber) return cpu;
        }

        return null;
    }

    /// <summary>
    /// チャレンジ状態にする
    /// </summary>
    public static void SetChallenge(int playerNumber)
    {
        if (_turn < 1) return;
        _isChallenge = true;
    }

    /// <summary>
    /// 自分の手番を終える
    /// </summary>
    public static void Advance()
    {
        _activeUserNumber++;
        if (_activeUserNumber >= OverAllManager.PlayerNumber)
        {
            AdvanceTurn();
            _activeUserNumber = 0;
        }
    }

    /// <summary>
    /// ターンを進める
    /// </summary>
    private static void AdvanceTurn()
    {
        _turn++;
    }

    /*
     * before challenge behavior
     */

    /// <summary>
    /// チャレンジを押した際
    /// </summary>
    private void Challenge()
    {
        bottomChallenge.SetActive(true);
        int cardCount = player.CardField.CardsCount;
        for (int i = 0; i < OverAllManager.PlayerNumber - 1; i++)
        {
            cardCount += cpus[i].CardField.CardsCount;
        }

        _maxFlowerChallengeNumber = cardCount;

        SetSlider(1, 1, cardCount);
    }

    /*
     * after challenge behavior
     */

    /// <summary>
    /// 決めたチャレンジ枚数をセット
    /// Player用
    /// </summary>
    public void Select()
    {
        _flowerChallengeNumber = (int) challengeNumberSetSlider.value;
        selectNumber.text = $"{_flowerChallengeNumber}枚";
        SetSlider(1, _flowerChallengeNumber + 1, _maxFlowerChallengeNumber);
        _challengePlayerNumber = 0;
        Advance();
    }

    public static void SetChallengeNumber(int flowerChallengeNumber,int playerNumber)
    {
        _flowerChallengeNumber = flowerChallengeNumber;
        _challengePlayerNumber = playerNumber;
    }

    /// <summary>
    /// スライダーの値各種セット
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    private void SetSlider(int value, int minValue, int maxValue)
    {
        challengeNumberSetSlider.value = value;
        challengeNumberSetSlider.minValue = minValue;
        challengeNumberSetSlider.maxValue = maxValue;
    }

    public static int Turn => _turn;

    public static bool IsChallenge => _isChallenge;

    public static int FlowerChallengeNumber => _flowerChallengeNumber;

    public static int MaxFlowerChallengeNumber => _maxFlowerChallengeNumber;
}