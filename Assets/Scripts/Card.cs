using System;
using System.Collections.Generic;
using DefaultNamespace;
using NUnit.Framework.Api;
using UnityEngine;

public class Card : MonoBehaviour
{
     private CardScriptableObject _cardScriptableObject;

     public int Row { get; set; }

     public event EventHandler<OnCardFlipEventArgs> OnCardFlip;

     [SerializeField] private bool faceUp = false;

     public class OnCardFlipEventArgs : EventArgs
     {
          public bool is_face_up;
     }

     public CardScriptableObject CardScriptableObject
     {
          get => _cardScriptableObject;
          set => _cardScriptableObject = value;
     }

     private void Start()
     {
     }

     private void Awake()
     {
          GameInput.Instance.OnCardClicked += OnCardClicked;
     }

     private void OnCardClicked(object sender, GameInput.OnCardClickedEventArgs e)
     {
          if (e.currentObject == gameObject)
          {

               if (e.action == CardActions.actions.selectCard)
               {
                    gameObject.GetComponent<UpdateSprite>().SetCardSelected(true);

                    if (IsTopCard())
                    {
                         FlipCard(true);
                    }

               }

               if (e.action == CardActions.actions.stackOnCard)
               {

                    if (e.previousObject.TryGetComponent<Card>(out Card card))
                    {

                         Card cardToMove = e.previousObject.GetComponent<Card>();
                         bool canBeStacked = CanBeStacked(cardToMove);

                         if (CanBeStacked(cardToMove) && cardToMove.faceUp)
                         {

                              if (cardToMove.IsTopCard() || cardToMove.isInDisplayedTrips())
                              {

                                   MoveCard(e.currentObject, e.previousObject);

                              }
                              else
                              {
                                   if (cardToMove.IsOnTableau())
                                   {
                                        
                                        MoveCardStack(e.currentObject, e.previousObject);

                                   }

                              }

                         }
                         else
                         {
                              e.previousObject.GetComponent<UpdateSprite>().SetCardSelected(false);
                              GameInput.Instance.PreviouslySelected = gameObject;
                              gameObject.GetComponent<UpdateSprite>().SetCardSelected(true);
                         }

                         if (IsTopCard())
                         {
                              FlipCard(true);
                         }
                    }
                    else if (!e.previousObject.TryGetComponent<Card>(out Card otherCard))
                    {
                         e.currentObject.GetComponent<UpdateSprite>().SetCardSelected(true);
                         GameInput.Instance.PreviouslySelected = gameObject;

                         if (IsTopCard())
                         {
                              FlipCard(true);
                         }

                    }
               }
          }


     }

     public bool IsTopCard()
     {

          int columnNumber = -1;
          int topCardIndex = -1;

          if (transform.parent.TryGetComponent<Foundation>(out Foundation foundation))
          {
               var foundations = GameManager.Instance.Foundations;

               columnNumber = foundation.GetColumnNumberListIndexFormat();
               topCardIndex = foundations[columnNumber].Count - 1;

               if (foundations[columnNumber][topCardIndex] == CardScriptableObject)
               {
                    return true;
               }

          }

          if (transform.parent.TryGetComponent<Tableau>(out Tableau tableau))
          {

               var tableaus = GameManager.Instance.Tableaus;

               columnNumber = tableau.GetColumnNumberListIndexFormat();
               topCardIndex = tableaus[columnNumber].Count - 1;

               if (tableaus[columnNumber][topCardIndex] == CardScriptableObject)
               {
                    return true;
               }

          }

          return false;
     }

     public void FlipCard(bool isFaceUp)
     {
          faceUp = isFaceUp;

          gameObject.GetComponent<UpdateSprite>().SetCardOrientation(faceUp);
     }

     private bool CanBeStacked(Card previousCard)
     {
          CardScriptableObject previousCardScriptableObject = previousCard.GetComponent<Card>().CardScriptableObject;

          if (faceUp)
          {
               //if the target card is on the foundation
               if (IsOnFoundation())
               {

                    if (_cardScriptableObject.Suite == previousCardScriptableObject.Suite)
                    {

                         if (_cardScriptableObject.numericValue() + 1 == previousCardScriptableObject.numericValue())
                         {

                              return true;

                         }

                    }

               }
               else
               {
                    //if the target card is on the tableau
                    if (!IsInDeck())
                    {

                         if (_cardScriptableObject.numericValue() - 1 == previousCardScriptableObject.numericValue())
                         {
                              if (_cardScriptableObject.getSuiteColor() != previousCardScriptableObject.getSuiteColor())
                              {
                                   if (IsTopCard())
                                   {
                                        return true;
                                   }
                              }
                         }
                    }
               }
          }

          return false;

     }

     public void MoveCard(GameObject moveTargetGameObject, GameObject cardBeingMovedGameObject)
     {
          Card cardBeingMovedCard = cardBeingMovedGameObject.GetComponent<Card>();
          GameManager.Instance.UpdateCardLists(moveTargetGameObject, cardBeingMovedGameObject);

          if (moveTargetGameObject.TryGetComponent<Tableau>(out Tableau tableau) ||
              moveTargetGameObject.TryGetComponent<Foundation>(out Foundation foundation))
          {
               moveTargetGameObject.TryGetComponent<Tableau>(out Tableau thisTableau);
               moveTargetGameObject.TryGetComponent<Foundation>(out Foundation thisFoundation);

               string nameOfMoveTarget = (thisFoundation != null) ? thisFoundation.ToString() : thisTableau.ToString();

               float zOffset = -0.5f;

               Vector3 childPosition = new Vector3(Vector3.zero.x, Vector3.zero.y, zOffset);

               cardBeingMovedGameObject.transform.SetParent(moveTargetGameObject.transform, false);

               cardBeingMovedGameObject.transform.localPosition = childPosition;

               cardBeingMovedGameObject.GetComponent<UpdateSprite>().SetCardSelected(false);
               GameInput.Instance.PreviouslySelected = null;

          }
          else
          {
               cardBeingMovedGameObject.transform.SetParent(gameObject.transform.parent, false);

               float yOffset = -0.75f;
               float zOffset = -0.5f;

               if (cardBeingMovedCard.IsOnFoundation())
               {
                    yOffset = 0.75f;
                    zOffset = -0.5f;
               }

               Vector3 childPosition = new Vector3(moveTargetGameObject.transform.localPosition.x,
                    moveTargetGameObject.transform.localPosition.y + yOffset,
                    moveTargetGameObject.transform.localPosition.z + zOffset);

               cardBeingMovedGameObject.transform.localPosition = childPosition;
               cardBeingMovedGameObject.GetComponent<UpdateSprite>().SetCardSelected(false);
               GameInput.Instance.PreviouslySelected = null;

          }

     }

     
     public void MoveCardStack(GameObject moveTargetGameObject, GameObject previousObject)
     {
          int indexOfCard = previousObject.transform.GetSiblingIndex();

          List<GameObject> stackToMove = new List<GameObject>();
          for (int i = indexOfCard; i < previousObject.transform.parent.childCount; i++)
          {

               stackToMove.Add(previousObject.transform.parent.GetChild(i).gameObject);

          }

          float yOffset =  -2.5f;
          float zOffset = -0.2f;
          float yOffsetIncrementValue = 0.5f;
          float zOffsetIncrementValue = 0.2f;

          foreach (GameObject cardGameObject in stackToMove)
          {

               Vector3 childPosition = new Vector3(moveTargetGameObject.transform.position.x,
                    moveTargetGameObject.transform.position.y + yOffset, moveTargetGameObject.transform.position.z + zOffset);

               cardGameObject.transform.localPosition = childPosition;
               cardGameObject.GetComponent<UpdateSprite>().SetCardSelected(false);

               yOffset += yOffsetIncrementValue;
               zOffset += zOffsetIncrementValue;

          }

          GameInput.Instance.PreviouslySelected = null;
     }

     public bool IsOnFoundation()
     {
          return IsOnOrIsFoundation(this.gameObject);
     }
     
     public bool IsOnOrIsFoundation(GameObject moveTargetGameObject)
     {
          if (moveTargetGameObject.TryGetComponent<Foundation>(out Foundation foundation) 
              || moveTargetGameObject.transform.parent.gameObject.TryGetComponent<Foundation>(out Foundation foundation2))
          {
               return true;
          } 

          return false;
     }


     

     public bool IsOnTableau()
     {
          if (IsInDeck())
          {
               return false;
          }

          if (gameObject.transform.parent.gameObject.TryGetComponent<Tableau>(out Tableau tableau))
          {
               return true;
          }

          return false;
     }


     public bool IsInDeck()
     {

          Debug.Log(CardScriptableObject.ToString());
          Debug.Log(this.transform.parent.gameObject.TryGetComponent<Stockpile>(out Stockpile stockpilex));

          return this.transform.parent.gameObject.TryGetComponent<Stockpile>(out Stockpile stockpile);
     }

     public bool isInDisplayedTrips()
     {
          
          return GameManager.Instance.IsInTripOnDisplay(this.CardScriptableObject);
     }


private void OnDestroy()
     {
          if (gameObject == GameInput.Instance.PreviouslySelected)
          {
               GameInput.Instance.PreviouslySelected = null;
          }
          GameInput.Instance.OnCardClicked -= OnCardClicked; 
          
     }
}
