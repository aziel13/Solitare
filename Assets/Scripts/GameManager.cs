using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [FormerlySerializedAs("cardPrefabs")] [SerializeField]
    private CardScriptableObject[] _CleanDeck;

    [SerializeField] private GameObject deckButton;

    public GameObject DeckButton => deckButton;

    [FormerlySerializedAs("bottomPos")] [SerializeField]
    private GameObject[] tableauPos;

    [FormerlySerializedAs("topPos")] [SerializeField]
    private GameObject[] foundationsPos;

    private List<CardScriptableObject>[] _tableaus;

    public List<CardScriptableObject>[] Tableaus => _tableaus;

    public List<CardScriptableObject>[] Foundations => _foundations;

    private List<CardScriptableObject>[] _foundations;
    private List<CardScriptableObject> _tripsOnDisplay = new List<CardScriptableObject>();
    private List<List<CardScriptableObject>> _deckTrips = new List<List<CardScriptableObject>>();

    private List<CardScriptableObject> _foundation0 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _foundation1 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _foundation2 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _foundation3 = new List<CardScriptableObject>();

    private List<CardScriptableObject> _tableau0 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _tableau1 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _tableau2 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _tableau3 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _tableau4 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _tableau5 = new List<CardScriptableObject>();
    private List<CardScriptableObject> _tableau6 = new List<CardScriptableObject>();

    private List<CardScriptableObject> _deck;

    private List<CardScriptableObject> _discardPile = new List<CardScriptableObject>();
    private int deckLocation;
    private int trips;
    private int tripsRemainder;

    public event EventHandler<OnPrintDebugMessageEventArgs> OnPrintDebugMessage;

    public class OnPrintDebugMessageEventArgs : EventArgs
    {
        public List<CardScriptableObject> deck;
        public List<CardScriptableObject> trips;
        public List<CardScriptableObject> discard;

        public List<CardScriptableObject>[] bottoms;
        public List<CardScriptableObject>[] tops;
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

        _tableaus = new List<CardScriptableObject>[]
            { _tableau0, _tableau1, _tableau2, _tableau3, _tableau4, _tableau5, _tableau6 };
        _foundations = new List<CardScriptableObject>[] { _foundation0, _foundation1, _foundation2, _foundation3 };

        GameInput.Instance.OnDeckClicked += Deck_OnDeckClicked;

        PlayCards();

    }

    private void Deck_OnDeckClicked(object sender, EventArgs e)
    {
        DealFromDeck();

        StartCoroutine(PrintCardListsToDebug());
    }

    public void PlayCards()
    {
        GenerateDeck();

        Shuffle(_deck);
        
        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        SortDeckIntoTrips();

        StartCoroutine(PrintCardListsToDebug());
    }

    public void UpdateCardLists(GameObject currentGameobject, GameObject previousGameObject)
    {
        Card cardScript = previousGameObject.GetComponent<Card>();
        //check to see if the currentGameObject is itself is a foundation or tableau
        bool currentIsAFoundation = currentGameobject.TryGetComponent<Foundation>(out Foundation foundation);
        bool currentIsATableau = currentGameobject.TryGetComponent<Tableau>(out Tableau tableau);
        //check to see if the current is a card
        bool currentIsACard = currentGameobject.TryGetComponent<Card>(out Card card);

        Foundation currentCardsFoundation = null;
        Tableau currentCardsTableau = null;
        //remove phase
        // is in the deck trip. Remove from the deck and trip lists. 

        //Debug.Log($"cardScript.IsInDeck(): {cardScript.IsInDeck()}");

        if (cardScript.IsInDeck())
        {

            RemoveFromDeck(cardScript.CardScriptableObject);
        }
        else
        {

            //check if the card is moving from the foundation or to the tableau
            if (cardScript.IsOnFoundation())
            {
                foreach (var topList in _foundations)
                {
                    if (topList.Contains(cardScript.CardScriptableObject))
                    {
                        topList.Remove(cardScript.CardScriptableObject);
                    }
                }
            }
            else
            {
                foreach (var topList in _tableaus)
                {
                    if (topList.Contains(cardScript.CardScriptableObject))
                    {
                        topList.Remove(cardScript.CardScriptableObject);
                    }
                }
            }
        }

        //Debug.Log($"currentIsAFoundation: {currentIsAFoundation} currentIsATableau: {currentIsATableau} currentIsACard: {currentIsACard}"); 

        if (currentIsACard)
        {

            if (card.IsOnFoundation())
            {
                currentCardsFoundation = card.transform.parent.gameObject.GetComponent<Foundation>();
                currentIsAFoundation = true;
            }
            else
            {
                currentCardsTableau = card.transform.parent.gameObject.GetComponent<Tableau>();
                currentIsATableau = true;
            }

        }

        //add phase
        //check if this is being moved to the foundation or the tablaeu
        if (currentIsAFoundation)
        {
            //Debug.Log($"currentCardsFoundation is null {currentCardsFoundation is null}"); 
            if (currentCardsFoundation is null)
            {
                currentCardsFoundation = foundation;
            }

            //Debug.Log($"Foundation Column Enum {currentCardsFoundation.GetFoundationPileValue()}"); 
            int columnNumber = 0;

            switch (currentCardsFoundation.GetFoundationPileValue())
            {
                default:
                case Foundation.FoundationColumns.ColumnOne:
                    break;
                case Foundation.FoundationColumns.ColumnTwo:
                    columnNumber = 1;
                    break;
                case Foundation.FoundationColumns.ColumnThree:
                    columnNumber = 2;
                    break;
                case Foundation.FoundationColumns.ColumnFour:
                    columnNumber = 3;
                    break;
            }

            _foundations[columnNumber].Add(cardScript.CardScriptableObject);

        }
        else if (currentIsATableau)
        {

            if (currentCardsTableau is null)
            {
                currentCardsTableau = tableau;
            }

            int columnNumber = 0;

            switch (currentCardsTableau.GetTableauPileValue())
            {
                default:
                case Tableau.TableauColumn.ColumnOne:
                    break;
                case Tableau.TableauColumn.ColumnTwo:
                    columnNumber = 1;
                    break;
                case Tableau.TableauColumn.ColumnThree:
                    columnNumber = 2;
                    break;
                case Tableau.TableauColumn.ColumnFour:
                    columnNumber = 3;
                    break;
                case Tableau.TableauColumn.ColumnFive:
                    columnNumber = 4;
                    break;
                case Tableau.TableauColumn.ColumnSix:
                    columnNumber = 5;
                    break;
                case Tableau.TableauColumn.ColumnSeven:
                    columnNumber = 6;
                    break;
            }

            _tableaus[columnNumber].Add(cardScript.CardScriptableObject);

        }

        StartCoroutine(PrintCardListsToDebug());

    }

    public Card GetCardOnTopOfThisCard(Card inCard)
    {
        // if this is the top card then there isn't a card on top of it
        if (inCard.IsTopCard())
        {
            return null;
        }

        //check where this card is.
        if (inCard.IsOnFoundation())
        {
            int foundationColumnIndex = inCard.transform.parent.gameObject.GetComponent<Foundation>()
                .GetColumnNumberListIndexFormat();
            // loop through the list excluding the top card
            for (int i = 0; i < _foundations[foundationColumnIndex].Count - 1; i++)
            {
                if (_foundations[foundationColumnIndex][i] == inCard.CardScriptableObject)
                {

                    CardScriptableObject cardScriptableObject = _foundations[foundationColumnIndex][i + 1];

                    //find this card in the incards parents child list



                    foreach (Transform child in inCard.transform.parent.transform)
                    {
                        if (child.TryGetComponent(out Card card))
                        {
                            if (cardScriptableObject.Suite == card.CardScriptableObject.Suite &&
                                cardScriptableObject.Value == inCard.CardScriptableObject.Value)
                            {
                                return card;
                            }
                        }
                    }
                }
            }

        }
        else if (inCard.IsOnTableau())
        {
            int tableauColumnIndex = inCard.transform.parent.gameObject.GetComponent<Tableau>()
                .GetColumnNumberListIndexFormat();
            //_tableaus
            for (int i = 0; i < _tableaus[tableauColumnIndex].Count - 1; i++)
            {
                if (_tableaus[tableauColumnIndex][i] == inCard.CardScriptableObject)
                {

                    CardScriptableObject cardScriptableObject = _tableaus[tableauColumnIndex][i + 1];

                    //find this card in the incards parents child list
                    foreach (Transform child in inCard.transform.parent.transform)
                    {
                        if (child.TryGetComponent(out Card card))
                        {
                            if (cardScriptableObject.Suite == card.CardScriptableObject.Suite &&
                                cardScriptableObject.Value == card.CardScriptableObject.Value)
                            {
                                return card;
                            }
                        }
                    }
                }
            }

        }

        //if this card is not on the foundation or the tableau it has no cards on top of it.
        return null;
    }


    public IEnumerator PrintCardListsToDebug()
    {
        yield return new WaitForSeconds(0.01f);
        OnPrintDebugMessage?.Invoke(this, new OnPrintDebugMessageEventArgs()
            {
                bottoms = _tableaus,
                tops = _foundations,
                deck = _deck,
                trips = _tripsOnDisplay,
                discard = _discardPile,

            }
        );
    }


    private void GenerateDeck()
    {
        _deck = new List<CardScriptableObject>();

        int index = 0;

        foreach (CardScriptableObject card in _CleanDeck)
        {
            _deck.Add(card);
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            int k = Next(n);
            n--;

            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    private int Next(int max)
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] fourBytes = new byte[4];
            rng.GetBytes(fourBytes);
            UInt32 scale = BitConverter.ToUInt32(fourBytes, 0);

            return (int)(0 + (max - 0) * (scale / (uint.MaxValue + 1.0)));
        }

    }

    private IEnumerator SolitaireDeal()
    {
        for (int i = 0; i < 7; i++)
        {

            float yOffset = 0;
            float zOffset = 0.03f;

            foreach (CardScriptableObject card in _tableaus[i])
            {
                yield return new WaitForSeconds(0.01f);
                Vector3 cardPos = tableauPos[i].transform.position;
                cardPos.y -= yOffset;
                cardPos.z -= zOffset;

                GameObject newCard = Instantiate(card.CardGameObject, cardPos, Quaternion.identity,
                    tableauPos[i].transform);
                newCard.name = card.ToString();

                newCard.GetComponent<UpdateSprite>().setFaceSprite(card.FaceSprite);

                Card cardScript = newCard.GetComponent<Card>();

                cardScript.Row = i;
                cardScript.CardScriptableObject = card;

                if (card == _tableaus[i][_tableaus[i].Count - 1])
                {
                    //replace direct reference with card event

                    cardScript.FlipCard(true);
                }


                yOffset += 0.3f;
                zOffset += 0.03f;
                _discardPile.Add(card);
            }

            foreach (CardScriptableObject card in _discardPile)
            {
                if (_deck.Contains(card))
                {
                    _deck.Remove(card);
                }
            }

            _discardPile.Clear();
        }
    }

    private void SolitaireSort()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                _tableaus[j].Add(_deck.Last<CardScriptableObject>());
                _deck.RemoveAt(_deck.Count - 1);
            }
        }
    }

    private void SortDeckIntoTrips()
    {
        trips = _deck.Count / 3;
        tripsRemainder = _deck.Count % 3;
        _deckTrips.Clear();

        int modifier = 0;

        for (int i = 0; i < trips; i++)
        {
            List<CardScriptableObject> myTrips = new List<CardScriptableObject>();
            for (int j = 0; j < 3; j++)
            {
                myTrips.Add(_deck[j + modifier]);
            }

            _deckTrips.Add(myTrips);
            modifier += 3;
        }

        if (tripsRemainder != 0)
        {
            List<CardScriptableObject> myRemainders = new List<CardScriptableObject>();
            modifier = 0;
            for (int i = 0; i < tripsRemainder; i++)
            {
                myRemainders.Add(_deck[^(tripsRemainder + modifier)]);
                modifier++;


            }

            _deckTrips.Add(myRemainders);
            trips++;
        }

        deckLocation = 0;
    }

    public void RemoveFromDeck(CardScriptableObject cardScriptableObjectToRemove)
    {

        // Debug.Log($"Remove from Trips on display");

        int itemsRemoved =
            _tripsOnDisplay.RemoveAll(cardScriptableObject => cardScriptableObject == cardScriptableObjectToRemove);

        //  Debug.Log($"itemsRemoved: {itemsRemoved}");
        
        //  Debug.Log($"Removing card from discard");

        for (int i =0 ; i< _deckTrips.Count; i++) {
            itemsRemoved = 
                _deckTrips[i].RemoveAll(cardScriptableObject => cardScriptableObject == cardScriptableObjectToRemove);

        }

        
        _discardPile.RemoveAll(cardScriptableObject => cardScriptableObject == cardScriptableObjectToRemove);

        // Debug.Log($"itemsRemoved: {itemsRemoved}");


        //  Debug.Log($"Removing card from deck");

        itemsRemoved = _deck.RemoveAll(cardScriptableObject => cardScriptableObject == cardScriptableObjectToRemove);


        StartCoroutine(PrintCardListsToDebug());
    }

    private void DealFromDeck()
    {

        List<CardScriptableObject> cardToRemove = new List<CardScriptableObject>();

        foreach (Transform child in deckButton.transform)
        {
            if (child.TryGetComponent(out Card card))
            {
                int itemsRemoved =
                    _deck.RemoveAll(cardScriptableObject => cardScriptableObject == card.CardScriptableObject);

                // Debug.Log($"itemsRemoved: {itemsRemoved}");

                _discardPile.Add(card.CardScriptableObject);

                Destroy(child.gameObject);
            }
        }


        if (deckLocation < trips)
        {
            _tripsOnDisplay.Clear();
            float xOffset = 2.5f;
            float zOffset = -0.2f;
            float xOffsetIncrementValue = 0.5f;
            float zOffsetIncrementValue = 0.2f;

            foreach (CardScriptableObject card in _deckTrips[deckLocation])
            {

                Vector3 cardPos = new Vector3(deckButton.transform.position.x + xOffset,
                    deckButton.transform.position.y, deckButton.transform.position.z + zOffset);

                GameObject newCard =
                    Instantiate(card.CardGameObject, cardPos, Quaternion.identity, deckButton.transform);
                newCard.name = card.ToString();
                newCard.GetComponent<Card>().CardScriptableObject = card;
                newCard.GetComponent<UpdateSprite>().CardFace = card.FaceSprite;
                xOffset += xOffsetIncrementValue;
                zOffset += zOffsetIncrementValue;
                _tripsOnDisplay.Add(card);
                newCard.GetComponent<Card>().FlipCard(true);

            }

            deckLocation++;

        }
        else
        {
            _tripsOnDisplay.Clear();

            RestockTopDeck();
        }

        removeCardsInPlayFromDeckTripsAndDiscard();
    }

    private void RestockTopDeck()
    {
        foreach (CardScriptableObject card in _discardPile)
        {
            _deck.Add(card);

        }

        _discardPile.Clear();
        SortDeckIntoTrips();
    }

    private bool isCardInPlay(CardScriptableObject card)
    {
        for (int i = 0; i < tableauPos.Length; i++)
        {
            for (int j = 0; j < tableauPos[i].transform.childCount; j++)
            {
                CardScriptableObject cardScript =  tableauPos[i].transform.GetChild(j).gameObject.GetComponent<Card>().CardScriptableObject;
                if (card == cardScript)
                {
                    return true;
                }
            }
        }
        
        for (int i = 0; i < foundationsPos.Length; i++)
        {
            for (int j = 0; j < foundationsPos[i].transform.childCount; j++)
            {
                CardScriptableObject cardScript =  foundationsPos[i].transform.GetChild(j).gameObject.GetComponent<Card>().CardScriptableObject;
                if (card == cardScript)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsInTripOnDisplay(CardScriptableObject card)
    {
        return _tripsOnDisplay.Contains(card);
    }

    private void removeCardsInPlayFromDeckTripsAndDiscard()
    {
        for (int i = 0; i < tableauPos.Length; i++)
        {
            for (int j = 0; j < tableauPos[i].transform.childCount; j++)
            {
                CardScriptableObject cardScript =  tableauPos[i].transform.GetChild(j).gameObject.GetComponent<Card>().CardScriptableObject;

                RemoveFromDeck(cardScript);
            }
        }
        
        for (int i = 0; i < foundationsPos.Length; i++)
        {
            for (int j = 0; j < foundationsPos[i].transform.childCount; j++)
            {
                CardScriptableObject cardScript =  foundationsPos[i].transform.GetChild(j).gameObject.GetComponent<Card>().CardScriptableObject;

                RemoveFromDeck(cardScript);
            }
        }
        
    }

}
