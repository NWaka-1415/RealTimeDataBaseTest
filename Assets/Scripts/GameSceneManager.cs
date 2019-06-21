using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    private bool _turn;

    [SerializeField] private CardField _cardField = null;
    [SerializeField] private Card[] _cards = new Card[4];

    private OverAllManager _overAllManager;

    private void Awake()
    {
        _turn = false;
        _overAllManager = GameObject.FindWithTag(OverAllManager.TagNames.GameController).GetComponent<OverAllManager>();
    }

    private void Start()
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].Initialize(OverAllManager.Card.CardTypes.Flower);
        }
    }

    private void Update()
    {
        if (!_turn) return;
    }
}