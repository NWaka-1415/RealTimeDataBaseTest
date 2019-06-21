using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    private OverAllManager.Card.CardTypes _cardType = OverAllManager.Card.CardTypes.Flower;
    private Vector2 _defaultPos;
    private bool _enable;
    [SerializeField] private RectTransform _rectTransform = null;

    public void Initialize(Vector2 pos, OverAllManager.Card.CardTypes cardType)
    {
        _enable = false;
        _cardType = cardType;
        _defaultPos = pos;
    }
    
    public void Initialize(OverAllManager.Card.CardTypes cardType)
    {
        _enable = false;
        _cardType = cardType;
        _defaultPos = _rectTransform.localPosition;
        Debug.Log(_defaultPos);
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!_enable) return;
        transform.position = eventData.position;
    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        _enable = true;
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_enable) return;
        _rectTransform.localPosition = _defaultPos;
    }

    /// <summary>
    /// ドロップ
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (RaycastResult hit in raycastResults)
        {
            // もし DroppableField の上なら、その位置に固定する
            if (hit.gameObject.CompareTag(OverAllManager.TagNames.DroppableField))
            {
                transform.position = hit.gameObject.transform.position;
                _enable = false;
                CardField cardField = hit.gameObject.GetComponent<CardField>();
                cardField.PlusCard();
            }
            else if (hit.gameObject.CompareTag(OverAllManager.TagNames.CardDefaultField))
            {
                _rectTransform.localPosition = _defaultPos;
                _enable = false;
            }
        }
    }
}