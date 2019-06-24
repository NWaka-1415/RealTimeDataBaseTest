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
    private int _activeUserNumber;

    /// <summary>
    /// あなたのプレイヤーの番号
    /// Your player number
    /// </summary>
    private int yourNumber;

    /// <summary>
    /// 現在のターン数
    /// Turn number on now
    /// </summary>
    private int _turn;

    [SerializeField] private CardField _cardField = null;
    [SerializeField] private Card[] _cards = new Card[4];

    [SerializeField] private CardField[] _cardFields = new CardField[5];

    [SerializeField] Sprite[] _cardSkullSprites = new Sprite[6];
    [SerializeField] Sprite[] _cardFlowerSprites = new Sprite[6];
    [SerializeField] Sprite[] _cardBackSprites = new Sprite[6];

    [SerializeField] private GameObject _bottomCard = null;
    [SerializeField] private GameObject _bottomChallenge = null;

    [SerializeField] private Text _challengeNumberText = null;
    [SerializeField] private Text _selectNumber = null;
    [SerializeField] private Slider _challengeNumberSetSlider = null;

    /// <summary>
    /// 全体管理スクリプト
    /// </summary>
    private OverAllManager _overAllManager;

    /// <summary>
    /// チャレンジのターンか
    /// Is the turn challenge turn?
    /// Yes=>true
    /// No=>false
    /// </summary>
    private bool _isChallenge;

    /// <summary>
    /// 宣言されているカード枚数
    /// </summary>
    private int _flowerChallengeNumber;

    private void Awake()
    {
        _activeUserNumber = 0;
        _isChallenge = false;
        _overAllManager = GameObject.FindWithTag(OverAllManager.TagNames.GameController).GetComponent<OverAllManager>();
        for (int i = OverAllManager.PlayerNumber - 1; i < _cardFields.Length; i++)
        {
            _cardFields[i].gameObject.SetActive(false);
        }

        _bottomCard.SetActive(true);
        _bottomChallenge.SetActive(false);
    }

    private void Start()
    {
        for (int i = 0; i < _cards.Length - 1; i++)
        {
            _cards[i].Initialize(OverAllManager.Card.CardTypes.Flower, _cardFlowerSprites[0], _cardBackSprites[0]);
            OverAllManager.YourUserData.HavingCards[i] = new OverAllManager.Card(_cards[i].CardType, _cards[i].State);
        }

        _cards[3].Initialize(OverAllManager.Card.CardTypes.Skull, _cardSkullSprites[0], _cardFlowerSprites[0]);
        OverAllManager.YourUserData.HavingCards[3] = new OverAllManager.Card(_cards[3].CardType, _cards[3].State);
    }

    private void Update()
    {
        if (_isChallenge)
        {
            _selectNumber.text = $"{_challengeNumberSetSlider.value}枚";
        }
    }

    /*
     * before challenge behavior
     */

    /// <summary>
    /// 自分の手番を終了
    /// </summary>
    public void MyTurnEnd()
    {
        if (_cardField.TurnCardNumber > 1)
        {
            
        }
        else
        {
            _activeUserNumber++;
            if (_activeUserNumber >= OverAllManager.PlayerNumber) _activeUserNumber = 0;
            _cardField.TurnEnd();
        }
    }

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