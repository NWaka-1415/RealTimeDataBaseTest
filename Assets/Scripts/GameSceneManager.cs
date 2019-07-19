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
    /// あなたのプレイヤーの番号
    /// Your player number
    /// </summary>
    private int _yourNumber;

    /// <summary>
    /// 現在のターン数
    /// Turn number on now
    /// </summary>
    private static int _turn;

    [SerializeField] private CardField _cardField = null;

    public static readonly Sprite[] CardSkullSprites = new Sprite[6];
    public static readonly Sprite[] CardFlowerSprites = new Sprite[6];
    public static readonly Sprite[] CardBackSprites = new Sprite[6];

    [SerializeField] private GameObject _bottomCard = null;
    [SerializeField] private GameObject _bottomChallenge = null;

    [SerializeField] private Text _challengeNumberText = null;
    [SerializeField] private Text _selectNumber = null;
    [SerializeField] private Slider _challengeNumberSetSlider = null;

    [SerializeField] private Button _challengeButton = null;
    [SerializeField] private Button _okButton = null;

    [SerializeField] private Player _player = null;

    [SerializeField] private CPU[] _cpus = new CPU[5];

    /// <summary>
    /// チャレンジのターンか
    /// Is the turn challenge turn?
    /// Yes=>true
    /// No=>false
    /// </summary>
    private static bool _isChallenge;

    /// <summary>
    /// 宣言されているカード枚数
    /// </summary>
    private int _flowerChallengeNumber;

    private void Awake()
    {
        _activeUserNumber = 0;
        _turn = 0;
        _isChallenge = false;
        _challengeButton.interactable = false;

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
        for (int i = OverAllManager.PlayerNumber - 1; i < _cpus.Length; i++)
        {
            // 不要なCPU分の場をHide
            _cpus[i].gameObject.SetActive(false);
        }

        _player.CardField.Initialize(OverAllManager.YourUserData);
        for (int i = 0; i < OverAllManager.PlayerNumber; i++)
        {
            _cpus[i].CardField.Initialize(new OverAllManager.UserData($"hoge{i}", $"hoge{i}"));
        }

        _bottomCard.SetActive(true);
        _bottomChallenge.SetActive(false);

        _okButton.onClick.AddListener(_player.Decide);
    }

    private void Update()
    {
        if (_turn > 0) _challengeButton.interactable = true;
        if (_isChallenge)
        {
            _selectNumber.text = $"{_challengeNumberSetSlider.value}枚";
        }

        _okButton.interactable = _activeUserNumber == 0;
    }

    public static void SetChallenge()
    {
        if (_turn < 1) return;
        _isChallenge = true;
    }

    public static void Advance()
    {
        _activeUserNumber++;
        if (_activeUserNumber >= OverAllManager.PlayerNumber) _activeUserNumber = 0;
    }

    /// <summary>
    /// ターン
    /// </summary>
    public static void AdvanceTurn()
    {
        _turn++;
    }

    /*
     * before challenge behavior
     */

    /// <summary>
    /// チャレンジを押した際
    /// </summary>
    public void Challenge()
    {
        _isChallenge = true;
        _bottomCard.SetActive(false);
        _bottomChallenge.SetActive(true);
        SetSlider(1, 1, 6);
    }

    /*
     * after challenge behavior
     */

    /// <summary>
    /// 決めたチャレンジ枚数をセット
    /// </summary>
    public void Select()
    {
        _flowerChallengeNumber = (int) _challengeNumberSetSlider.value;
        _challengeNumberText.text = $"{_flowerChallengeNumber}枚";
    }

    /// <summary>
    /// パス
    /// pass
    /// </summary>
    public void Pass()
    {
        _activeUserNumber++;
        if (_activeUserNumber >= OverAllManager.PlayerNumber) _activeUserNumber = 0;
    }

    /// <summary>
    /// スライダーの値各種セット
    /// </summary>
    /// <param name="value"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    private void SetSlider(int value, int minValue, int maxValue)
    {
        _challengeNumberSetSlider.value = value;
        _challengeNumberSetSlider.minValue = minValue;
        _challengeNumberSetSlider.maxValue = maxValue;
    }
}