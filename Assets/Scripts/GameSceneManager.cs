using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public enum GameModeTypes
    {
        PushCardMode, //初期。場にカードを出す
        SelectChallengeNumberMode, //チャレンジ開始！何枚花のカードがあるかを決める
        SelectCardFromOtherPlayerMode //数が決まったらドクロを選ばないようにカードを選ぶ
    }

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
    [SerializeField] private GameObject bottomSelectField = null;

    [SerializeField] private Text challengeNumberText = null;
    [SerializeField] private Text selectNumber = null;
    [SerializeField] private Slider challengeNumberSetSlider = null;

    [SerializeField] private Button challengeButton = null;
    [SerializeField] private Button okButtonOnCards = null;

    [SerializeField] private Button okButtonOnChallenge = null;
    [SerializeField] private Button passButton = null;

    [SerializeField] private Text selectCardNumberText = null;
    [SerializeField] private Button okButtonOnSelectCard = null;

    [SerializeField] private Player player = null;

    [SerializeField] private CPU[] cpus = new CPU[5];

    private static List<CardField> _cardFields = new List<CardField>();

    /// <summary>
    /// 今ゲームはどの場面か
    /// </summary>
    private static GameModeTypes _gameModeType;

    private GameModeTypes _prevGameModeType;

    /// <summary>
    /// 宣言されているカード枚数
    /// </summary>
    private static int _flowerChallengeNumber;

    /// <summary>
    /// チャレンジできる最大カード枚数
    /// </summary>
    private static int _maxFlowerChallengeNumber;

    /// <summary>
    /// 展開されているカードの枚数
    /// </summary>
    private static int _openedCardsNumber;

    /// <summary>
    /// ターン間の待ち時間(CPU)
    /// </summary>
    private float _timeToTurnAndTurn;

    private void Awake()
    {
        Initialize();

        _activeUserNumber = 0;
        _turn = 0;

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
            cpus[i].Initialize(new OverAllManager.UserData($"hoge{i}", $"hoge{i}"), player);
        }

        SetPlayer();
        player.SetCards();
        _cardFields.Add(player.CardField);
        for (int i = 0; i < OverAllManager.PlayerNumber - 1; i++)
        {
            cpus[i].SetCards(false);
            _cardFields.Add(cpus[i].CardField);
        }

        okButtonOnCards.onClick.AddListener(player.DecideSetCard);
        challengeButton.onClick.AddListener(player.Challenge);

        okButtonOnChallenge.onClick.AddListener(Select);
        passButton.onClick.AddListener(player.Pass);

        okButtonOnSelectCard.onClick.AddListener(player.DecideOpenCard);
        okButtonOnSelectCard.gameObject.SetActive(false);
    }

    private void Update()
    {
        switch (_gameModeType)
        {
            case GameModeTypes.PushCardMode:
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

                break;
            case GameModeTypes.SelectChallengeNumberMode:
                Debug.Log($"act:{_activeUserNumber}\nchallenge:{_challengePlayerNumber}");
                if (_activeUserNumber == _challengePlayerNumber)
                {
                    _gameModeType = GameModeTypes.SelectCardFromOtherPlayerMode;
                    for (int i = 0; i < OverAllManager.PlayerNumber - 1; i++)
                    {
                        cpus[i].SetSelectThisButton();
                    }

                    break;
                }

                if (_prevGameModeType != _gameModeType && _gameModeType == GameModeTypes.SelectChallengeNumberMode)
                {
                    Challenge();
                    _prevGameModeType = _gameModeType;
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

                break;
            case GameModeTypes.SelectCardFromOtherPlayerMode:
                //チャレンジ権利を持つプレイヤーに準場が回ってきた際
                bottomChallenge.SetActive(false);
                bottomSelectField.SetActive(true);

                okButtonOnSelectCard.interactable = _activeUserNumber == 0;
                selectCardNumberText.text = $"{_openedCardsNumber} / {_flowerChallengeNumber}枚";

                if (_activeUserNumber != 0)
                {
                    //CPUs
                    if (_timeToTurnAndTurn <= 0f)
                    {
                        FindActiveCPU(_activeUserNumber).Thinking();
                        selectCardNumberText.text = $"{_openedCardsNumber} / {_flowerChallengeNumber}枚";
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
                else
                {
                    if (player.IsClear || player.IsOut)
                    {
                        //スカルが出たか、ポイントを取ったら終了
                        player.ResetThis();
                        for (int i = 0; i < OverAllManager.PlayerNumber - 1; i++)
                        {
                            cpus[i].ResetThis();
                        }

                        Initialize();

                        break;
                    }

                    player.OpenAllMyCards();
                    selectCardNumberText.text = $"{_openedCardsNumber} / {_flowerChallengeNumber}枚";
                }

                break;
        }
    }

    private void Initialize()
    {
        _turn = 0;
        _timeToTurnAndTurn = 1f;
        _flowerChallengeNumber = 0;
        _challengePlayerNumber = -1;
        _openedCardsNumber = 0;
        _gameModeType = GameModeTypes.PushCardMode;
        _prevGameModeType = _gameModeType;
        bottomCard.SetActive(true);
        bottomChallenge.SetActive(false);
        bottomSelectField.SetActive(false);
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
        _sayChallengePlayerNumber = playerNumber;
        _gameModeType = GameModeTypes.SelectChallengeNumberMode;
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
        int minValue = _flowerChallengeNumber + 1;
        if (minValue >= _maxFlowerChallengeNumber) minValue = _maxFlowerChallengeNumber;
        SetSlider(1, minValue, _maxFlowerChallengeNumber);
        _challengePlayerNumber = 0;
        Advance();
    }

    /// <summary>
    /// 決めたチャレンジ枚数をセット
    /// CPU用
    /// </summary>
    /// <param name="flowerChallengeNumber"></param>
    /// <param name="playerNumber"></param>
    public static void SetChallengeNumber(int flowerChallengeNumber, int playerNumber)
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
        if (minValue >= maxValue)
        {
            okButtonOnChallenge.interactable = false;
            challengeNumberSetSlider.interactable = false;
        }
    }

    public static int Turn => _turn;

    public static GameModeTypes GameModeType => _gameModeType;

    public static int FlowerChallengeNumber => _flowerChallengeNumber;

    public static int ChallengePlayerNumber => _challengePlayerNumber;

    public static int MaxFlowerChallengeNumber => _maxFlowerChallengeNumber;

    public static int OpenedCardsNumber
    {
        get => _openedCardsNumber;
        set => _openedCardsNumber = value;
    }

    public static List<CardField> CardFields => _cardFields;
}