using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : MonoBehaviour
{
    [SerializeField] private CardField _cardField = null;
    [SerializeField] private Card[] _flowerCards = null;
    [SerializeField] private Card _skullCard = null;

    private int flowerIndex = 0;

    private void Awake()
    {
        _skullCard.gameObject.SetActive(false);
        _skullCard.Initialize(cardType: OverAllManager.Card.CardTypes.Skull,
            front: GameSceneManager.CardSkullSprites[0], back: GameSceneManager.CardBackSprites[0],
            isYours: false);
        foreach (Card flowerCard in _flowerCards)
        {
            flowerCard.gameObject.SetActive(false);
            flowerCard.Initialize(cardType: OverAllManager.Card.CardTypes.Flower,
                front: GameSceneManager.CardFlowerSprites[0], back: GameSceneManager.CardBackSprites[0],
                isYours: false);
        }
    }

    /// <summary>
    /// どのカード出そうかな
    /// </summary>
    public void SelectCard()
    {
        float rand = Random.Range(0f, 1f);
        float per = 1f / _flowerCards.Length + 1;

        if (rand < per)
        {
            _skullCard.gameObject.SetActive(true);
            _skullCard.transform.position = _cardField.DropPos;
            _skullCard.transform.SetAsLastSibling();
        }
        else
        {
            _flowerCards[flowerIndex].gameObject.SetActive(true);
            _flowerCards[flowerIndex].transform.position = _cardField.DropPos;
            _flowerCards[flowerIndex].transform.SetAsLastSibling();
            flowerIndex++;
        }
    }

    public CardField CardField => _cardField;
}