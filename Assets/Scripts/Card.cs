using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    private OverAllManager.Card _card;
    private Vector2 _defaultPos;
    [SerializeField] private Image _image = null;
    private Sprite _frontSprite;
    private Sprite _backSprite;

    /// <summary>
    /// ドロップ出来るか
    /// </summary>
    private bool _enable;

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
    [SerializeField] private RectTransform _rectTransform = null;

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
        _enable = false;
        _decided = false;
        _frontSprite = front;
        _backSprite = back;
        _defaultPos = pos;
        _isYours = isYours;
        if (_isYours) _image.sprite = _frontSprite;
        else _image.sprite = _backSprite;
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
        Initialize(_rectTransform.localPosition, cardType, front, back, isYours);
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
        _enable = false;
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

    /// <summary>
    /// ドラッグ中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isYours) return;
        if (!_enable) return;
        transform.position = eventData.position;
    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isYours) return;
        if (_decided) return;
        _enable = true;
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isYours) return;
        if (!_enable) return;
        _rectTransform.localPosition = _defaultPos;
    }

    /// <summary>
    /// ドロップ
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        if (!_isYours) return;
        if (!_enable) return;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (RaycastResult hit in raycastResults)
        {
            Debug.Log($"{this.gameObject.name} hits {hit.gameObject.name}");
            // もし DroppableField の上なら、その位置に固定する
            if (hit.gameObject.CompareTag(OverAllManager.TagNames.DroppableField))
            {
                CardField cardField = hit.gameObject.GetComponent<CardField>();
                transform.position = cardField.DropPos;
                _rectTransform.SetAsLastSibling();
                _enable = false;
                cardField.PlusCard(this);
            }
            else if (hit.gameObject.CompareTag(OverAllManager.TagNames.CardDefaultField))
            {
                _rectTransform.localPosition = _defaultPos;
                _enable = false;
            }
        }
    }
}