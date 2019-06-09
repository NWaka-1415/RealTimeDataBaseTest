using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    private OverAllManager.Card.CardTypes _cardType = OverAllManager.Card.CardTypes.Flower;
    private Vector2 _defaultPos;
    private bool _enable;

    public void Initialize(Vector2 pos, OverAllManager.Card.CardTypes cardType)
    {
        _enable = false;
        _cardType = cardType;
        _defaultPos = pos;
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
        _defaultPos = transform.position;
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_enable) return;
        transform.position = _defaultPos;
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
            if (hit.gameObject.CompareTag("DroppableField"))
            {
                transform.position = hit.gameObject.transform.position;
                _enable = false;
            }
        }
    }
}