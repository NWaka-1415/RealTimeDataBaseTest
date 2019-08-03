using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private OverAllManager.Card _card;
    private Vector2 _defaultPos;
    private Player _player = null;
    [SerializeField] private Image image = null;
    [SerializeField] private Image selectImage = null;
    [SerializeField] private Button button = null;
    private bool _isSelected;
    private Sprite _frontSprite;
    private Sprite _backSprite;

    /// <summary>
    /// 決定されたカードか否か
    /// true:決定済み
    /// </summary>
    private bool _decided;

    /// <summary>
    /// プレイヤーの持っているカードかどうか
    /// プレイヤーのもの=>true
    /// CPU or 対戦相手=>false
    /// </summary>
    private bool _isYours;

    /// <summary>
    /// 短径トランスフォーム
    /// </summary>
    [SerializeField] private RectTransform rectTransform = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="cardType"></param>
    /// <param name="back"></param>
    /// <param name="front"></param>
    /// <param name="isYours"></param>
    public void Initialize(Vector2 pos, OverAllManager.Card.CardTypes cardType, Sprite front, Sprite back,
        bool isYours = true)
    {
        _card = new OverAllManager.Card(cardType);
        _decided = false;
        selectImage.enabled = false;
        _isSelected = false;
        _frontSprite = front;
        _backSprite = back;
        _defaultPos = pos;
        _isYours = isYours;
        if (_isYours) button.onClick.AddListener(OnclickSelect);
        else button.enabled = false;
        image.sprite = _isYours ? _frontSprite : _backSprite;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="cardType"></param>
    /// <param name="back"></param>
    /// <param name="front"></param>
    /// <param name="isYours"></param>
    public void Initialize(OverAllManager.Card.CardTypes cardType, Sprite front, Sprite back, bool isYours = true)
    {
        Initialize(rectTransform.localPosition, cardType, front, back, isYours);
    }

    /// <summary>
    /// プレイヤーをセット
    /// </summary>
    /// <param name="player"></param>
    public void SetParentPlayer(Player player)
    {
        _player = player;
//        Debug.Log($"Player:{player}");
    }

    /// <summary>
    /// クリックにより選択
    /// </summary>
    public void OnclickSelect()
    {
        _player.Select(this);
    }

    /// <summary>
    /// 選択
    /// </summary>
    public void Select()
    {
        if(_decided) return;
        _isSelected = true;
        selectImage.enabled = _isSelected;
    }

    /// <summary>
    /// 非選択
    /// </summary>
    public void Unselect()
    {
        if(_decided) return;
        _isSelected = false;
        selectImage.enabled = _isSelected;
    }

    /// <summary>
    /// 決定済み
    /// </summary>
    public void SetDisable()
    {
        _decided = true;
    }

    /// <summary>
    /// カードリセット
    /// </summary>
    public void ResetCard()
    {
        _card.Close();
    }

    /// <summary>
    /// カードを開く
    /// </summary>
    public void Open()
    {
        _card.Open();
    }

    public OverAllManager.Card.States State => _card.State;

    public OverAllManager.Card.CardTypes CardType => _card.CardType;
}